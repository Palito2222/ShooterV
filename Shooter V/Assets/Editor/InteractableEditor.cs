using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        base.OnInspectorGUI();
        if (interactable.useEvents)
        {
            //Si estamos usando Eventos, añadir el componente InteractionEvent
            if (interactable.GetComponent<InteractionEvent>() == null) 
            {
                interactable.gameObject.AddComponent<InteractionEvent>();
            }
        }
        else
        {
            //Si no estamos usando eventos, eliminar el componente InteractionEvent
            if(interactable.GetComponent<InteractionEvent>() != null) 
            {
                DestroyImmediate(interactable.GetComponent<InteractionEvent>());
            }
        }
    }
}
