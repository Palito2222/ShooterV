using UnityEngine;

public class CursorVisibility : MonoBehaviour
{
    // Variable para almacenar el estado actual de visibilidad del cursor
    private bool cursorVisible = true;

    private void Update()
    {
        // Verificar si se ha presionado la tecla "P"
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Alternar la visibilidad del cursor
            cursorVisible = !cursorVisible;

            // Establecer la visibilidad del cursor en función del estado actual
            Cursor.visible = cursorVisible;

            // Bloquear o desbloquear el cursor según su visibilidad
            Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
