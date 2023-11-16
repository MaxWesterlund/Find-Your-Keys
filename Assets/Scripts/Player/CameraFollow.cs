using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] Transform target;

    Vector3 targetOffset;

    void Start() {
        targetOffset = transform.position - target.position;
    }

    void Update() {
        transform.position = target.position + targetOffset;
    }
}
