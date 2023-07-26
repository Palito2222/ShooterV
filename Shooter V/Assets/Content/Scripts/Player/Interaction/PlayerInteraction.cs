using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    private CinemachineVirtualCamera Vcam;

    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private GameObject eventSystem;

    private PlayerUI playerUI;
    private PlayerInput playerInput;

    public override void OnStartLocalPlayer()
    {
        eventSystem.SetActive(true);
        Vcam = GetComponent<PlayerLook>().VCam;
        playerUI = GetComponent<PlayerUI>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        playerUI.UpdateText(string.Empty);

        //Creamos un Ray en el centro de la cámara, el cual irá hacia delante. Esto detecta colisiones.
        Ray ray = new Ray(Vcam.transform.position, Vcam.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * distance);

        RaycastHit hitInfo; //Variable para almacenar la información de la colisión

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>()!= null)
            {
                //Recortamos un poco el código para que sea más legible
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

                playerUI.UpdateText(interactable.promptMessage);
                if (playerInput.MovementActions.Interact.triggered)
                {
                    interactable.BaseInteract(gameObject);
                }
            }
        }
    }
}
