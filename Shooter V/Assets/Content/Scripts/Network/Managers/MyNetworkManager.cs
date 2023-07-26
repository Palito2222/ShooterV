using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance;
    private PlayerManager playerManager;

    public override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //Tenemos una referencia al PlayerManager instance
        playerManager = PlayerManager.instance;

        NetworkIdentity connID = conn.identity;

        //Creamos una variable temporal que es igual al ID de la nueva conexi�n
        int playerConnectionId = conn.connectionId;

        GameObject playerGO = conn.identity.gameObject;
        //A�adimos a un nuevo jugador al Diccionario de PlayerManager, con el ID de dicha conexi�n
        playerManager.AddPlayer(playerConnectionId, 100, playerGO, connID);

        // Llamar al m�todo TargetReceivePlayerID en el PlayerController correspondiente
        PlayerController playerController = conn.identity.GetComponent<PlayerController>();
        playerController.TargetReceivePlayerID(playerConnectionId);
    }

    // Nuevo m�todo para imprimir los nombres de los spawnable prefabs
    public void PrintSpawnablePrefabs()
    {
        Debug.Log("Spawnable Prefabs:");
        foreach (GameObject prefab in spawnPrefabs)
        {
            Debug.Log(prefab.name);
        }
    }
}

