using Mirror;
using System.Collections;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float destroyTime;
    [SerializeField] LayerMask mask;

    // Variable para almacenar el ID del jugador atacante
    public int AttackerID { get; set; }

    private bool hasHit = false; // Variable para evitar que la bala aplique da�o m�ltiples veces

    private void Start()
    {
        // Llamamos a la corrutina para destruir la bala despu�s de un tiempo determinado
        StartCoroutine(DestroyBulletCooldown());
    }

    private void Update()
    {
        BulletMove();
    }

    public void BulletMove()
    {
        // Obt�n la direcci�n en la que se mueve la bala (seg�n su transform.forward)
        Vector3 direction = transform.forward;

        // Mueve la bala en la direcci�n establecida a una velocidad determinada
        transform.position += direction * speed * Time.deltaTime;
    }

    // M�todo para detectar colisiones
    void OnTriggerEnter(Collider other)
    {
        if (!hasHit & isOwned)
        {
            // Comprueba si el objeto con el que colision� est� en la capa "Player"
            if (mask == (mask | (1 << other.gameObject.layer)))
            {
                // Obtiene el componente PlayerController del jugador impactado
                PlayerController playerController = other.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    // Obtiene el NetworkIdentity del jugador impactado
                    NetworkIdentity targetIdentity = other.GetComponent<NetworkIdentity>();

                    if (targetIdentity != null && targetIdentity.connectionToClient != null)
                    {
                        // Obtiene el ID del jugador impactado
                        int playerID = targetIdentity.connectionToClient.connectionId;

                        // Llama a la funci�n que maneja el da�o del jugador
                        PlayerManager.instance.DamagePlayer(playerID, damage, AttackerID);
                    }
                    else
                    {
                        Debug.LogWarning("El jugador impactado no tiene una conexi�n v�lida.");
                    }
                }
            }

            // Destruye la bala despu�s de impactar a un jugador
            DestroyBulletServer();
        }
    }

    IEnumerator DestroyBulletCooldown()
    {
        yield return new WaitForSeconds(destroyTime);

        // Si la bala no ha impactado con nada despu�s del tiempo determinado, la destruye
        if (!hasHit & isOwned)
        {
            DestroyBulletServer();
        }
    }

    [Command]
    void DestroyBulletServer()
    {
        // Destruye la bala en el servidor y sincroniza la destrucci�n con los clientes autom�ticamente
        NetworkServer.Destroy(gameObject);
    }
}
