using Cinemachine;
using Mirror;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    public GameObject playerGO;
    public GameObject cinemachineVcam;
    public CinemachineVirtualCamera VCam;
    private float xRotation = 0f;

    public float xSensitivity;
    public float ySensitivity;

    public override void OnStartLocalPlayer()
    {
        cinemachineVcam.SetActive(true);
    }

    public void Look(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        //Calcular la rotación de cámara para mirar arriba y debajo
        xRotation -= mouseY * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        //Aplicamos lo anterior al Transform de la cámara
        VCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Rotar al jugador a la derecha o izquierda
        playerGO.transform.Rotate(Vector3.up * mouseX * xSensitivity);
    }
}
