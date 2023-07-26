using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    private List <Transform> spawnPoint;

    public readonly SyncDictionary<int, Player> players = new SyncDictionary<int, Player>();
    public readonly SyncDictionary<int, NetworkIdentity> connectionsDict = new SyncDictionary<int, NetworkIdentity>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [Server]
    public void AddPlayer(int playerID, int pHealth, GameObject playerGO, NetworkIdentity connClient)
    {
        if (!players.ContainsKey(playerID))
        {
            players.Add(playerID, new PlayerManager.Player() { health = pHealth, gameObject = playerGO, connID = playerID });
            connectionsDict.Add(playerID, connClient); // Agregar la conexión al diccionario
            Debug.Log("Se añadió a la lista a: " + playerID + ", Su ConnID: " + players[playerID].connID);
        }
    }

    [Server]
    public void DamagePlayer(int playerID, int damage, int attackerID)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.Log("¡El jugador no está en el diccionario!");
            return;
        }

        players[playerID].health -= damage;

        Debug.Log("Player " + playerID.ToString() + " health is " + players[playerID].health);

        if (players[playerID].health <= 0)
        {
            players[playerID].health = 0;
            PlayerKilled(playerID, attackerID);
        }

        // Llama al comando para actualizar la salud en PlayerHealth
        RpcUpdatePlayerHealth(players[playerID].health, players[playerID].gameObject);
    }

    [ClientRpc]
    private void RpcUpdatePlayerHealth(int newHealth, GameObject playerGO)
    {
        // Busca el objeto PlayerHealth local con el ID del jugador y actualiza la salud
        PlayerHealth playerHealth = playerGO.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetHealth(newHealth);
        }
    }

    // Método para obtener la vida actual de un jugador por su ID
    public int GetPlayerHealth(int playerID)
    {
        if (players.ContainsKey(playerID))
        {
            return players[playerID].health;
        }
        else
        {
            Debug.LogError("Player not found in PlayerManager dictionary!");
            return 0;
        }
    }

    [Server]
    private void PlayerKilled(int playerID, int AttackerID)
    {
        string deathText = "Player: " + AttackerID + " ha eliminado a: " + playerID;
        RpcUpdateDeathMessageForAllPlayers(deathText);

        // Disparar el evento de muerte
        players[playerID].deaths++;
        players[playerID].health = 100;
        RpcUpdatePlayerHealth(players[playerID].health, players[playerID].gameObject);
        players[AttackerID].kills++;

        NetworkIdentity conn = connectionsDict[playerID];
        NetworkConnectionToClient connClient = conn.connectionToClient;
        RespawnPlayer(connClient, players[playerID].gameObject, Random.Range(0, spawnPoint.Count));
    }

    [ClientRpc]
    private void RpcUpdateDeathMessageForAllPlayers(string newText)
    {
        foreach (var connection in connectionsDict)
        {
            if (connection.Value != null)
            {
                PlayerUI playerUI = connection.Value.GetComponent<PlayerUI>();
                if (playerUI != null)
                {
                    playerUI.UpdateDeathMessage(newText);
                    playerUI.StartCoroutine(playerUI.ClearDeathMessage());
                }
            }
        }
    }

    [TargetRpc]
    private void RespawnPlayer(NetworkConnectionToClient conn, GameObject player, int spawn)
    {
        player.transform.position = spawnPoint[spawn].position;
    }

    [System.Serializable]
    public class Player
    {
        public string name;

        public int health;

        public int kills;

        public int deaths;

        public GameObject gameObject;

        public int connID;
    }
}
