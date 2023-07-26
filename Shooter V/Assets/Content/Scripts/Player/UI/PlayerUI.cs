using UnityEngine;
using TMPro;
using Mirror;
using System.Collections;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI prompText;

    [SerializeField]
    private TextMeshProUGUI playerID;

    [SerializeField]
    private TextMeshProUGUI deathMessage;

    public void UpdateText(string prompMessage)
    {
        prompText.text = prompMessage;
    }

    public void UpdatePlayerID(string playerIDTxT)
    {
        playerID.text = "ID: " + playerIDTxT;
    }

    public void UpdateDeathMessage(string newText)
    {
        deathMessage.text = newText;
    }

    public IEnumerator ClearDeathMessage()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(5f);

        deathMessage.text = "";
    }
}
