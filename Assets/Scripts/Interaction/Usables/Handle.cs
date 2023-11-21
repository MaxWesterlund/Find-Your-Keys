using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : Usable {
    MeshCollider collider;

    [SerializeField] Transform hinge;
    [SerializeField] float openAngle;
    [SerializeField] float openingDuration;
    [SerializeField] AnimationCurve openingCurve;

    float lastOpenTime;
    float previousAngle;
    float nextAngle;
    float angle;

    bool isOpen;

    void Awake() {
        collider = GetComponentInParent<MeshCollider>();
    }

    void Update() {
        if (lastOpenTime > Time.time) {
            float progress = 1 - (lastOpenTime - Time.time) / openingDuration;
            collider.isTrigger = true;
            angle = Mathf.Lerp(previousAngle, nextAngle, openingCurve.Evaluate(progress));
        }
        else {
            collider.isTrigger = false;
        }
        hinge.eulerAngles = Vector3.up * angle;
    }

    public override void Use(GameObject item) {
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        Transform root = transform.root;
        if (isOpen) {
            isOpen = false;
            previousAngle = nextAngle;
            nextAngle = 0;
        }
        else {
            isOpen = true;
            previousAngle = nextAngle;
            if (root.forward == Vector3.forward) {
            nextAngle = playerPos.z > root.position.z ? -openAngle : openAngle;
            }
            else if (root.forward == Vector3.right) {
                nextAngle = playerPos.x > root.position.x ? -openAngle : openAngle;
            }
            else if (root.forward == Vector3.back) {
                nextAngle = playerPos.z > root.position.z ? openAngle : -openAngle;
            }
            else if (root.forward == Vector3.left) {
                nextAngle = playerPos.x > root.position.x ? openAngle : -openAngle;
            }
        }
        lastOpenTime = Time.time + openingDuration;
    }
}
