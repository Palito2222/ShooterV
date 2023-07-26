using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WatermarkController : MonoBehaviour
{
    // Variable para determinar si es una build de desarrollo o no
    public bool isDevelopmentBuild = true;

    // Referencia al objeto Text que muestra la marca de agua
    public TextMeshProUGUI watermarkText;

    // Texto de la marca de agua que se mostrar� en las builds de desarrollo
    public string developmentBuildText = "Development Build";

    // Texto de la marca de agua que se mostrar� en las builds oficiales
    public string officialBuildText = "";

    private void Start()
    {
        if (isDevelopmentBuild)
        {
            // Mostrar el texto de desarrollo
            watermarkText.text = developmentBuildText;
        }
        else
        {
            // Mostrar el texto de la versi�n oficial
            watermarkText.text = officialBuildText;
        }
    }
}

