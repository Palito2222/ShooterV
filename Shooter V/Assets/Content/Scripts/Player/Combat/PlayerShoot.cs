using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private float fireRate = 0.3f;
    [SerializeField]
    private WaitForSeconds shootWait;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private GameObject prefabBullet;
    [SerializeField]
    private float distanceSpawnBullet;

    private bool canShoot = true;

    private PlayerLook look;
    private PlayerController playerController;

    public override void OnStartLocalPlayer()
    {
        look = GetComponent<PlayerLook>();
        playerController = GetComponent<PlayerController>();

        //Establecemos que shootWait es una espera que depende del fireRate
        shootWait = new WaitForSeconds(fireRate);
    }

    public void Shoot()
    {
        //Si canShoot es True, entonces disparamos e inicia una coroutina.
        if (canShoot & isLocalPlayer) 
        {
            // Obt�n el ID del jugador atacante
            int attackerID = playerController.GetPlayerID();

            // Calcular la posici�n de instanciaci�n de la bala
            Vector3 spawnPosition = look.VCam.transform.position + look.VCam.transform.forward * distanceSpawnBullet;

            //Llamamos un Cmd para que se genere el GameObject
            CmdShoot(attackerID, spawnPosition, look.VCam.transform.rotation);

            StartCoroutine(CanShootUpdater());
        }
    }

    [Command]
    void CmdShoot(int attackerID, Vector3 spawnPosition, Quaternion rotationVcam)
    {
        if (spawnPosition != null) 
        {
            // Acceder al prefab de la bala desde el NetworkManager
            GameObject bulletPrefab = MyNetworkManager.instance.spawnPrefabs.FirstOrDefault(prefab => prefab.name == "Bullet");
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotationVcam);

            // Asignar el ID del jugador atacante a la bala
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.AttackerID = attackerID;


            NetworkIdentity connClient = PlayerManager.instance.connectionsDict[attackerID];

            NetworkServer.Spawn(bullet, connClient.connectionToClient);
            RpcOnShoot(bullet);
        }
        else
        {
            Debug.LogError("BulletPrefab o spawnPosition es nulo.");
        }
    }

    [ClientRpc]
    void RpcOnShoot(GameObject bullet)
    {
        // Instanciar la bala tambi�n en los clientes
        if (isLocalPlayer)
        {
            // El objeto ya est� instanciado en el servidor y fue sincronizado autom�ticamente con los clientes.
            // Si eres el jugador local, aseg�rate de que tambi�n lo veas.
            // (esto puede no ser necesario dependiendo de c�mo tienes configurada tu c�mara y vista del jugador)
        }
    }

    //La coroutina esperar� el fireRate que hemos puesto, y no podremos disparar hasta que haya pasado dicho tiempo.
    private IEnumerator CanShootUpdater()
    {
        canShoot = false;

        yield return shootWait;

        canShoot = true;
    }
}
