using UnityEngine;

public class FPSUI : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style;

    private void Awake()
    {
        // Configura el estilo del texto para mostrar los FPS
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = 24;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        // Calcula el tiempo de diferencia entre cuadros
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        // Calcula los FPS
        float fps = 1.0f / deltaTime;

        // Obtiene las dimensiones de la pantalla
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Muestra los FPS en la esquina superior derecha de la pantalla
        GUI.Label(new Rect(screenWidth - 210, 10, 200, 40), "FPS: " + Mathf.RoundToInt(fps), style);
    }
}
