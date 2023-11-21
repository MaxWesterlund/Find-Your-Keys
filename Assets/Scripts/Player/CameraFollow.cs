using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] Transform follow;
    [SerializeField] float lookAngle;
    [SerializeField] float lookRotation;
    [SerializeField] float distance;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    Vector3 targetOffset;

    void Start() {
        transform.position = GetTargetPosition();
    }

    void Update() {
        Vector3 targetPosition = GetTargetPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(follow.position - GetTargetPosition());
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    Vector3 GetTargetPosition() {
        float angle = lookAngle * Mathf.Deg2Rad;
        float rotation = lookRotation * Mathf.Deg2Rad;
        float horizontalDistance = distance * Mathf.Cos(angle);
        float x = horizontalDistance * Mathf.Cos(rotation);
        float y = distance * Mathf.Sin(angle);
        float z = horizontalDistance * Mathf.Sin(rotation);
        Vector3 pos = follow.position + new Vector3(x, y, z);
        return pos;
    }
}
