using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    CharacterController controller;

    [SerializeField] float speed;

    Vector3 lastMoveDirection;
    Vector3 moveDirection;

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Start() {
        PlayerInputManager.Instance.SubPerformed("Move", OnMove);
    }

    void FixedUpdate() {
        controller.Move(speed * Time.fixedDeltaTime * moveDirection);
        controller.Move(10 * Vector3.down);
        Vector3 rotDir = moveDirection == Vector3.zero ? lastMoveDirection : moveDirection;
        Quaternion targetRotation = Quaternion.LookRotation(rotDir);
        if (moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.fixedDeltaTime);
        }
    }

    void OnMove(InputAction.CallbackContext ctx) {
        Vector2 val = ctx.ReadValue<Vector2>();
        lastMoveDirection = moveDirection;
        moveDirection = new Vector3(val.x, 0, val.y);
    }
}
