using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JugadorController : MonoBehaviour
{
    [Header("Configuración de Movimiento y Salto")]
    public float velocidad = 6f;
    public float fuerzaSalto = 7f;
    public float gravedad = -20f;

    private CharacterController controller;
    private float velocidadVertical;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded && velocidadVertical < 0)
        {
            velocidadVertical = -2f;
        }

        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1;

            // Saltar con Shift Izquierdo o tecla E
            if ((Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame) && controller.isGrounded)
            {
                velocidadVertical = fuerzaSalto;
            }
        }

        Vector3 movimiento = new Vector3(input.x, 0, input.y).normalized;

        if (movimiento.magnitude >= 0.1f)
        {
            controller.Move(movimiento * velocidad * Time.deltaTime);
            transform.forward = movimiento;
        }

        velocidadVertical += gravedad * Time.deltaTime;
        controller.Move(Vector3.up * velocidadVertical * Time.deltaTime);
    }
}