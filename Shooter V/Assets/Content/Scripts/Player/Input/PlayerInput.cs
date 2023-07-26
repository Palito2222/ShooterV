using Mirror;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
    public PlayerInputActions InputActions { get; private set; }
    public PlayerInputActions.MovementActions MovementActions { get; private set; }
    public PlayerInputActions.CombatActions CombatActions { get; private set; }

    private PlayerController player;
    private PlayerLook look;
    private PlayerShoot shoot;

    public override void OnStartLocalPlayer()
    {
        //Se crea una instancia nueva de PlayerInputActions
        InputActions = new PlayerInputActions();

        //Asignamos las acciones del PlayerInputActions a la nueva instancia
        MovementActions = InputActions.Movement;

        //Asignamos las acciones del PlayerInputActions a la nueva instancia
        CombatActions = InputActions.Combat;

        //Habilitamos las acciones
        InputActions.Enable();

        //Asignamos el Script del player
        player = GetComponent<PlayerController>();
        //Asignamos el Script del PlayerLook
        look = GetComponent<PlayerLook>();
        //Asignamos el Script del PlayerLook
        shoot = GetComponent<PlayerShoot>();

        //Nos suscribimos a la acci�n de Salto - Usamos un Callback Context para llamar al m�todo
        MovementActions.Jump.performed += ctx => player.OnJump();

        //Nos suscribimos a la acci�n de Agacharse - Usamos un Callback Context para llamar al m�todo
        MovementActions.Crouch.performed += ctx => player.Crouch();

        //Nos suscribimos a la acci�n de Sprint - Usamos un Callback Context para llamar al m�todo
        MovementActions.Sprint.performed += ctx => player.Sprint();

        //Nos suscribimos a la acci�n de Shoot - Usamos un Callback Context para llamar al m�todo
        CombatActions.Shoot.performed += ctx => shoot.Shoot();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        //Le decimos al PlayerController que mueva usando el valor del MovementAction
        player.OnMove(MovementActions.Move.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;

        //Le decimos a la c�mara que mueva usando el valor de la acci�n de Look
        look.Look(MovementActions.Look.ReadValue<Vector2>());
    }

    //M�todo Reusable para activar las acciones de movimiento
    public void OnEnableMovement()
    {
        MovementActions.Enable();
    }

    //M�todo Reusable para desactivar las acciones de movimiento
    public void OnDisableMovement()
    {
        MovementActions.Disable();
    }

    //M�todo Reusable para activar las acciones de combate
    public void OnEnableCombat()
    {
        CombatActions.Enable();
    }

    //M�todo Reusable para desactivar las acciones de combate
    public void OnDisableCombat()
    {
        CombatActions.Disable();
    }

    //M�todo Reusable para desactivar una acci�n
    public void DisableActionFor(InputAction action, float seconds)
    {
        StartCoroutine(DisableAction(action, seconds));
    }

    private IEnumerator DisableAction(InputAction action, float seconds)
    {
        action.Disable();

        yield return new WaitForSeconds(seconds);

        action.Enable();
    }
}
