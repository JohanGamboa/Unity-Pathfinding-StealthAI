using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaMovil : MonoBehaviour
{
    [Header("Desplazamiento al presionar Espacio")]
    public Vector3 desplazamientoCerrado = new Vector3(0, 0, 0);
    public Vector3 desplazamientoAbierto = new Vector3(-3.5f, 0, 0);
    public float velocidad = 4f;

    private Vector3 posicionInicial;
    private bool estaAbierta = false;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            estaAbierta = !estaAbierta;
        }

        Vector3 destino = posicionInicial + (estaAbierta ? desplazamientoAbierto : desplazamientoCerrado);
        transform.position = Vector3.Lerp(transform.position, destino, velocidad * Time.deltaTime);
    }
}