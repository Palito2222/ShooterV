using Mirror;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    //Añade o elimina un componente InteractionEvent del GameObject
    public bool useEvents;

    //Mensaje que saldrá cuando el jugador mire al objeto interactuable
    public string promptMessage;

    //Esta función será llamada por nuestro Jugador (script)
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
        //Este método es una Template para que que sea sobreescrita por las subclases
    }
}
