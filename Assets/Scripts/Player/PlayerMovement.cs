using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    CharacterController controller;

    [SerializeField] float speed;
    [SerializeField] float acceleration;

    Vector3 lastMoveDirection;
    Vector3 moveDirection;

    float moveAmount;

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Start() {
        PlayerInputManager.Instance.SubPerformedAndCanceled("Move", OnMove);
    }

    void Update() {
        if (moveDirection != Vector3.zero) {
            moveAmount += acceleration * Time.deltaTime;
        }
        else {
            moveAmount -= acceleration * Time.deltaTime;
        }
        moveAmount = Mathf.Clamp01(moveAmount);
        float movement = speed * moveAmount * Time.deltaTime;
        if (moveDirection == Vector3.zero) {
            controller.Move(movement * controller.velocity.normalized);
        }
        else {
            controller.Move(movement * moveDirection);
        }
        controller.Move(10 * Vector3.down);
        Vector3 dir = moveDirection == Vector3.zero ? lastMoveDirection : moveDirection;
        Quaternion targetRotation = dir == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(dir);
        if (moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void OnMove(InputAction.CallbackContext ctx) {
        Vector2 val = ctx.ReadValue<Vector2>();
        lastMoveDirection = moveDirection;
        float angle = Mathf.Atan2(val.y, val.x);
        moveDirection = val.magnitude * new Vector3(Mathf.Cos(angle + Mathf.PI / 4), 0, Mathf.Sin(angle + Mathf.PI * val.magnitude / 4));
    }
}
