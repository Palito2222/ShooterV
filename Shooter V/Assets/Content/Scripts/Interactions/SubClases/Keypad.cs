using Mirror;
using UnityEngine;

public class Keypad : Interactable
{
    [SerializeField]
    private GameObject door;
    private bool doorOpen;

    //En este método, es donde vamos a diseñar la interacción usando código
    protected override void Interact(GameObject interactingPlayer)
    {
        if ( isOwned ) 
        {
            PlayAnimation();
            return;
        }
        else if ( !isOwned )
        {
            AssignAuthority(interactingPlayer);
            return;
        }
    }

    [Command(requiresAuthority = false)]
    private void AssignAuthority(GameObject interactingPlayer)
    {
        // Asignar autoridad al objeto en el servidor
        // Obtengo el Identity del GameObject actual
        NetworkIdentity keypadIdentity = GetComponent<NetworkIdentity>();

        // Obtengo el PlayerID del Jugador que interactuó
        int playerID = interactingPlayer.GetComponent<PlayerController>().GetPlayerID();
        // Obtengo del Diccionario de Conexiones, el NetworkConnection del Jugador en base a su ID en el diccionario
        NetworkConnectionToClient playerIdentity = PlayerManager.instance.connectionsDict[playerID].connectionToClient;
        //Asigno la autoridad
        keypadIdentity.AssignClientAuthority(playerIdentity);

        PlayAnimation();
    }

    [Command]
    private void PlayAnimation()
    {
        doorOpen = !doorOpen;
        door.GetComponent<NetworkAnimator>().animator.SetBool("isOpen", doorOpen);
    }

    public void DestroyKeypad()
    {
        //Prueba de Evento
        Destroy(this.gameObject);
    }
}
