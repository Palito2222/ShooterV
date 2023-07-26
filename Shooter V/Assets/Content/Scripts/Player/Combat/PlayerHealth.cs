using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] // Utilizamos un hook para detectar cambios en la vida del jugador
    private int health;

    private float lerpTimer;
    public int maxHealth = 100;
    public float chipSpeed = 2f;

    [SerializeField]
    private GameObject UIGO;

    public Image frontHealthBar;
    public Image backHealthBar;

    private PlayerManager playerManager;
    private PlayerController playerController;

    public override void OnStartLocalPlayer()
    {
        playerManager = PlayerManager.instance;
        playerController = GetComponent<PlayerController>();
        UIGO.SetActive(true);

        // Obtén la vida actual del jugador desde el PlayerManager
        health = PlayerManager.instance.GetPlayerHealth(playerController.GetPlayerID());
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Limita el valor de health entre 0 y maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHealthUI();
    }

    #region UpdateHealthUI + Server - Client

    public void SetHealth(int newHealth)
    {
        if (!isLocalPlayer) { return; }

        health = newHealth;
        lerpTimer = 0;
    }

    public void UpdateHealthUI()
    {
        if (!isLocalPlayer) { return; }

        // Obtiene el valor actual de relleno de las barras de salud frontal y trasera
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;

        // Calcula la fracción de salud actual con respecto a la salud máxima
        float hFraction = (float)health / maxHealth;

        // Si la barra de salud trasera tiene más relleno que la fracción de salud actual,
        // realiza una animación de transición para ajustar el relleno a la nueva cantidad de salud.
        if (fillBack > hFraction)
        {
            // Ajusta la barra frontal a la nueva fracción de salud
            frontHealthBar.fillAmount = hFraction;

            // Cambia el color de la barra trasera a rojo para indicar una disminución en la salud
            backHealthBar.color = Color.red;

            // Inicializa el temporizador de interpolación para la animación
            lerpTimer += Time.deltaTime;

            // Calcula el porcentaje de completado de la interpolación
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;

            // Interpola el relleno de la barra trasera desde su valor actual hacia la nueva fracción de salud
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        // Si la barra de salud delantera tiene menos relleno que la fracción de salud actual,
        // realiza una animación de transición para ajustar el relleno a la nueva cantidad de salud.
        if (fillFront < hFraction)
        {
            // Cambia el color de la barra trasera a verde para indicar un aumento en la salud
            backHealthBar.color = Color.green;

            // Ajusta la barra frontal a la nueva fracción de salud
            backHealthBar.fillAmount = hFraction;

            // Inicializa el temporizador de interpolación para la animación
            lerpTimer += Time.deltaTime;

            // Calcula el porcentaje de completado de la interpolación
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;

            // Interpola el relleno de la barra delantera desde su valor actual hacia la nueva fracción de salud
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
        }
    }

    #endregion

    #region RestoreHealth + Server - Client

    // Método para restaurar la salud del jugador
    public void RestoreHealth(int healthAmount)
    {
        int playerID = GetComponent<NetworkIdentity>().connectionToClient.connectionId;

        // Asegúrate de que el jugador esté registrado en el diccionario
        if (playerManager.players.ContainsKey(playerID))
        {
            // Modificar la salud del jugador
            playerManager.players[playerID].health += healthAmount;
            lerpTimer = 0f;
        }
        else
        {
            Debug.LogError("Player not found in PlayerManager dictionary!");
        }
    }
    #endregion
}
