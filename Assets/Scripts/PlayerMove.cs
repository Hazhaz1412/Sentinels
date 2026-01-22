using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerMove : NetworkBehaviour
{
    private void Update()
    {
        if (isLocalPlayer)
        {
            InputAction move = InputSystem.actions.FindAction("Move");
            Vector2 moveValue = move.ReadValue<Vector2>();

            Vector3 playerMovement = new Vector3(moveValue.x * 0.25f, moveValue.y * 0.25f, 0);

            transform.position = transform.position + playerMovement;
        }
    }
}