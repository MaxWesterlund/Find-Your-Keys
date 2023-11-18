using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalk : MonoBehaviour {
    [SerializeField] Transform center;
    [SerializeField] Transform leftTarget;
    [SerializeField] Transform rightTarget;

    [SerializeField] float minStepLength;
    [SerializeField] float maxStepLength;

    Vector3 lastCenterPos;
    Vector3 lastLeftTargetPos;
    Vector3 lastRightTargetPos;

    void Update() {
        leftTarget.position = lastLeftTargetPos;
        rightTarget.position = lastRightTargetPos;

        if (!IsInBalanceBox()) {
            float leftDist = Vector2.Distance(AsVec2(leftTarget.position), AsVec2(center.position));
            float rightDist = Vector2.Distance(AsVec2(rightTarget.position), AsVec2(center.position));
            Transform move = leftDist > rightDist ? leftTarget : rightTarget;

            Vector3 dir = (center.position - lastCenterPos).normalized;
            float footDist = Mathf.Clamp(Vector2.Distance(AsVec2(leftTarget.position), AsVec2(rightTarget.position)), minStepLength, maxStepLength);
            Vector3 moveAmnt = 2 * footDist * dir;
            move.position += moveAmnt;
        }

        lastCenterPos = Ground(center.position);
        lastLeftTargetPos = Ground(leftTarget.position);
        lastRightTargetPos = Ground(rightTarget.position);
    }

    bool IsInBalanceBox() {
        float upperLimit = Mathf.Max(leftTarget.position.z, rightTarget.position.z);
        float lowerLimit = Mathf.Min(leftTarget.position.z, rightTarget.position.z);
        float leftLimit = Mathf.Min(leftTarget.position.x, rightTarget.position.x);
        float rightLimit = Mathf.Max(leftTarget.position.x, rightTarget.position.x);
        if (center.position.z > upperLimit || center.position.z < lowerLimit || center.position.x < leftLimit || center.position.x > rightLimit) {
            return false;
        }
        return true;
    }

    Vector2 AsVec2(Vector3 vec) {
        return new Vector2(vec.x, vec.z);
    }

    Vector3 Ground(Vector3 vec) {
        return new Vector3(vec.x, 0, vec.z);
    }
}
