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

    private bool hasHit = false; // Variable para evitar que la bala aplique daño múltiples veces

    private void Start()
    {
        // Llamamos a la corrutina para destruir la bala después de un tiempo determinado
        StartCoroutine(DestroyBulletCooldown());
    }

    private void Update()
    {
        BulletMove();
    }

    public void BulletMove()
    {
        // Obtén la dirección en la que se mueve la bala (según su transform.forward)
        Vector3 direction = transform.forward;

        // Mueve la bala en la dirección establecida a una velocidad determinada
        transform.position += direction * speed * Time.deltaTime;
    }

    // Método para detectar colisiones
    void OnTriggerEnter(Collider other)
    {
        if (!hasHit & isOwned)
        {
            // Comprueba si el objeto con el que colisionó está en la capa "Player"
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

                        // Llama a la función que maneja el daño del jugador
                        PlayerManager.instance.DamagePlayer(playerID, damage, AttackerID);
                    }
                    else
                    {
                        Debug.LogWarning("El jugador impactado no tiene una conexión válida.");
                    }
                }
            }

            // Destruye la bala después de impactar a un jugador
            DestroyBulletServer();
        }
    }

    IEnumerator DestroyBulletCooldown()
    {
        yield return new WaitForSeconds(destroyTime);

        // Si la bala no ha impactado con nada después del tiempo determinado, la destruye
        if (!hasHit & isOwned)
        {
            DestroyBulletServer();
        }
    }

    [Command]
    void DestroyBulletServer()
    {
        // Destruye la bala en el servidor y sincroniza la destrucción con los clientes automáticamente
        NetworkServer.Destroy(gameObject);
    }
}
