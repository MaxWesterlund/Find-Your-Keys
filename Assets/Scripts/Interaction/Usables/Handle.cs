using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : Usable {
    MeshCollider meshCollider;

    [SerializeField] Transform door;
    [SerializeField] Transform hinge;
    [SerializeField] float minOpenAngle;
    [SerializeField] float maxOpenAngle;
    [SerializeField] float openingDuration;
    [SerializeField] AnimationCurve openingCurve;

    float lastInteractTime;
    float previousAngle;
    float nextAngle;
    float angle;

    bool isOpen;

    void Awake() {
        meshCollider = GetComponentInParent<MeshCollider>();
    }

    void Update() {
        if (lastInteractTime > Time.time) {
            float progress = 1 - (lastInteractTime - Time.time) / openingDuration;
            meshCollider.isTrigger = true;
            angle = Mathf.Lerp(previousAngle, nextAngle, openingCurve.Evaluate(progress));
        }
        else {
            meshCollider.isTrigger = false;
        }

        hinge.localEulerAngles = Vector3.up * angle;
    }

    public override void Use(GameObject item) {
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        if (isOpen) {
            isOpen = false;
            previousAngle = nextAngle;
            nextAngle = 0;
        }
        else {
            isOpen = true;
            previousAngle = nextAngle;
            float angle = Random.Range(minOpenAngle, maxOpenAngle);
            if (door.forward == Vector3.forward) {
                nextAngle = playerPos.z > door.position.z ? -angle : angle;
            }
            else if (door.forward == Vector3.right) {
                nextAngle = playerPos.x > door.position.x ? -angle : angle;
            }
            else if (door.forward == Vector3.back) {
                nextAngle = playerPos.z > door.position.z ? angle : -angle;
            }
            else if (door.forward == Vector3.left) {
                nextAngle = playerPos.x > door.position.x ? angle : -angle;
            }
        }
        lastInteractTime = Time.time + openingDuration;
    }
}
