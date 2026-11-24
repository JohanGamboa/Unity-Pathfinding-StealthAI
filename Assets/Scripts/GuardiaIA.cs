using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class GuardiaIA : MonoBehaviour
{
    public enum EstadoIA { Patrullando, Persiguiendo, Investigando }
    [Header("Estado Actual (Solo lectura)")]
    public EstadoIA estadoActual = EstadoIA.Patrullando;

    [Header("Referencias")]
    public Transform[] waypoints;
    public Transform ojos;
    private Transform jugador;
    private NavMeshAgent agente;
    private LineRenderer lineaRuta;

    [Header("Parámetros de Detección y Velocidad")]
    public float rangoVision = 11f;
    [Range(0, 180)] public float anguloVision = 75f;
    public float velocidadPatrulla = 3.5f;
    public float velocidadPersecucion = 5.5f;
    public float tiempoInvestigacion = 3f;

    private int indiceWaypoint = 0;
    private Vector3 ultimaPosicionConocida;
    private float temporizadorBusqueda = 0f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        lineaRuta = GetComponent<LineRenderer>();

        // Configurar la línea visual que muestra el cálculo A* en tiempo real
        lineaRuta.startWidth = 0.12f;
        lineaRuta.endWidth = 0.12f;
        lineaRuta.material = new Material(Shader.Find("Sprites/Default"));
        lineaRuta.startColor = Color.cyan;
        lineaRuta.endColor = Color.yellow;

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;

        IrAlSiguienteWaypoint();
    }

    void Update()
    {
        bool veAlJugador = DetectarJugador();

        switch (estadoActual)
        {
            case EstadoIA.Patrullando:
                agente.speed = velocidadPatrulla;
                if (veAlJugador)
                {
                    estadoActual = EstadoIA.Persiguiendo;
                }
                else if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
                {
                    IrAlSiguienteWaypoint();
                }
                break;

            case EstadoIA.Persiguiendo:
                agente.speed = velocidadPersecucion;
                if (veAlJugador && jugador != null)
                {
                    ultimaPosicionConocida = jugador.position;
                    agente.SetDestination(jugador.position);
                }
                else
                {
                    // Perdió de vista al jugador: va a investigar el último punto donde lo vio
                    agente.SetDestination(ultimaPosicionConocida);
                    temporizadorBusqueda = tiempoInvestigacion;
                    estadoActual = EstadoIA.Investigando;
                }
                break;

            case EstadoIA.Investigando:
                agente.speed = velocidadPatrulla;
                if (veAlJugador)
                {
                    estadoActual = EstadoIA.Persiguiendo;
                }
                else if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
                {
                    temporizadorBusqueda -= Time.deltaTime;
                    // Gira buscando al jugador
                    transform.Rotate(0, 90f * Time.deltaTime, 0);

                    if (temporizadorBusqueda <= 0f)
                    {
                        estadoActual = EstadoIA.Patrullando;
                        IrAlSiguienteWaypoint();
                    }
                }
                break;
        }

        DibujarRutaAStar();
    }

    void IrAlSiguienteWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        agente.SetDestination(waypoints[indiceWaypoint].position);
        indiceWaypoint = (indiceWaypoint + 1) % waypoints.Length;
    }

    bool DetectarJugador()
    {
        if (jugador == null || ojos == null) return false;

        Vector3 direccionHaciaJugador = (jugador.position - ojos.position).normalized;
        float distancia = Vector3.Distance(ojos.position, jugador.position);

        if (distancia <= rangoVision)
        {
            float angulo = Vector3.Angle(transform.forward, direccionHaciaJugador);
            if (angulo <= anguloVision * 0.5f)
            {
                // Raycast para comprobar que no haya una pared bloqueando la vista
                if (Physics.Raycast(ojos.position, direccionHaciaJugador, out RaycastHit hit, rangoVision))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.DrawLine(ojos.position, hit.point, Color.red);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void DibujarRutaAStar()
    {
        if (agente.hasPath)
        {
            lineaRuta.positionCount = agente.path.corners.Length;
            lineaRuta.SetPositions(agente.path.corners);
        }
        else
        {
            lineaRuta.positionCount = 0;
        }
    }

    // Dibuja el cono de visión en la ventana Scene para el video y capturas
    void OnDrawGizmos()
    {
        if (ojos == null) return;
        Gizmos.color = estadoActual == EstadoIA.Persiguiendo ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(ojos.position, rangoVision);

        Vector3 limiteIzq = Quaternion.Euler(0, -anguloVision * 0.5f, 0) * transform.forward * rangoVision;
        Vector3 limiteDer = Quaternion.Euler(0, anguloVision * 0.5f, 0) * transform.forward * rangoVision;

        Gizmos.DrawRay(ojos.position, limiteIzq);
        Gizmos.DrawRay(ojos.position, limiteDer);
    }
}