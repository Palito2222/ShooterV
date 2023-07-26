using Mirror;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    //A�ade o elimina un componente InteractionEvent del GameObject
    public bool useEvents;

    //Mensaje que saldr� cuando el jugador mire al objeto interactuable
    public string promptMessage;

    //Esta funci�n ser� llamada por nuestro Jugador (script)
    public void BaseInteract(GameObject interactingPlayer)
    {
        if (useEvents) 
        {
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        }

        Interact(interactingPlayer);
    }

    protected virtual void Interact(GameObject interactingPlayer)
    {
        //Este m�todo es una Template para que que sea sobreescrita por las subclases
    }
}
