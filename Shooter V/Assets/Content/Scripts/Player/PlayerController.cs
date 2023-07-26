using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    CharacterController characterController;
    PlayerInput input;
    PlayerUI playerUI;

    //Variables de Movimiento y Salto
    private float moveSpeed;
    public float baseSpeed = 7.5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    Vector3 velocity;
    bool isGrounded;

    //Variables de Agachado
    bool crouching = false;
    float crouchTimer = 1;
    bool lerpCrouch = false;

    //Variables de Sprint
    bool sprinting = false;

    private int playerID;

    public override void OnStartLocalPlayer()
    {
        characterController = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        playerUI = GetComponent<PlayerUI>();

        moveSpeed = baseSpeed;
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

    [TargetRpc]
    public void TargetReceivePlayerID(int PlayerID)
    {
        playerID = PlayerID;
        Debug.Log("PlayerID: " + playerID);
        playerUI.UpdatePlayerID(playerID.ToString());
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerManager playerManager = PlayerManager.instance;

            if (playerManager != null)
            {
                foreach (var playerEntry in playerManager.players)
                {
                    int playerID = playerEntry.Key;
                    PlayerManager.Player player = playerEntry.Value;

                    // Aquí puedes hacer lo que necesites con cada jugador en la lista
                    Debug.Log("Player ID: " + playerID + ", Player Health: " + player.health);
                }
            }
            else
            {
                Debug.Log("playerManager es null");
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MyNetworkManager networkManager = MyNetworkManager.instance;

            if (networkManager != null)
            {
                networkManager.PrintSpawnablePrefabs();
            }
            else
            {
                Debug.Log("networkManager es null");
            }
        }

        isGrounded = characterController.isGrounded;

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
            {
                characterController.height = Mathf.Lerp(characterController.height, 1, p);
            }
            else
            {
                characterController.height = Mathf.Lerp(characterController.height, 1.73f, p);
            }

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0;
            }
        }
    }

    public void OnMove(Vector2 input)
    {
        //Convertimos el movimiento 2D en uno 3D
        Vector3 moveDirection = GetMovementInputDirection(input);

        // Mover el personaje
        Physics.SyncTransforms();
        characterController.Move(transform.TransformDirection(moveDirection) * moveSpeed * Time.smoothDeltaTime);

        // Aplicar gravedad al personaje
        velocity.y += gravity * Time.smoothDeltaTime;

        // Verificar si el personaje está en el suelo y ajustar la velocidad vertical
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Physics.SyncTransforms();
        // Mover el personaje en función de la velocidad vertical
        characterController.Move(velocity * Time.smoothDeltaTime);
    }


    public void OnJump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = baseSpeed;
        }
    }

    #region Reusable Methods

    //Convertir movimiento 2D a 3D
    public Vector3 GetMovementInputDirection(Vector2 input)
    {
        return new Vector3(input.x, 0f, input.y);
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    #endregion
}
