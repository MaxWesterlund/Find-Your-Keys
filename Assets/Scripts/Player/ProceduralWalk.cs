using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalk : MonoBehaviour {
    [Header("Transforms")]
    [SerializeField] Transform center;
    [SerializeField] Transform leftTarget;
    [SerializeField] Transform rightTarget;
    
    [Header("Step Settings")]
    [SerializeField] float balanceRadius;
    [SerializeField] float footSpacing;
    [SerializeField] float stepTime;
    [SerializeField] float stepHeight;

    [Header("Audio")]
    [SerializeField] float minVolume;
    [SerializeField] float maxVolume;
    [SerializeField] AudioSource leftFootSound;
    [SerializeField] AudioSource rightFootSound;

    Vector3 lastBalancePoint;
    Vector3 nextLeftPosition;
    Vector3 nextRightPosition;
    Vector3 lastLeftGroundPosition;
    Vector3 lastRightGroundPosition;

    float leftLeaveGroundTime;
    float rightLeaveGroundTime;

    void Start() {
        nextLeftPosition = center.position - center.right * footSpacing / 2;
        nextRightPosition = center.position + center.right * footSpacing / 2;
        leftTarget.position = nextLeftPosition;
        rightTarget.position = nextRightPosition;
    }

    void Update() {
        float leftProgress = 1 - Mathf.Clamp(leftLeaveGroundTime + stepTime - Time.time, 0, stepTime) / stepTime;
        float rightProgress = 1 - Mathf.Clamp(rightLeaveGroundTime + stepTime - Time.time, 0, stepTime) / stepTime;

        leftTarget.position = Vector3.Lerp(lastLeftGroundPosition, nextLeftPosition, leftProgress) + Mathf.Sin(leftProgress * Mathf.PI) * stepHeight * Vector3.up;
        rightTarget.position = Vector3.Lerp(lastRightGroundPosition, nextRightPosition, rightProgress) + Mathf.Sin(rightProgress * Mathf.PI) * stepHeight * Vector3.up;

        float leftDist = Vector2.Distance(new Vector2(leftTarget.position.x, leftTarget.position.z), new Vector2(center.position.x, center.position.z));
        float rightDist = Vector2.Distance(new Vector2(rightTarget.position.x, rightTarget.position.z), new Vector2(center.position.x, center.position.z));
        bool shouldMoveLeft = leftDist > rightDist;
        
        if (Vector3.Distance(center.position, lastBalancePoint) > balanceRadius) {
            TakeStep(shouldMoveLeft);
        }
    }

    void TakeStep(bool shouldMoveLeft) {
        Vector3 dir = (center.position - lastBalancePoint) / balanceRadius;
        float angle = Mathf.Atan2(dir.z, dir.x);
        float offsetAngle = angle + (shouldMoveLeft ? 1 : -1) * 90;
        Vector3 offset = footSpacing / 2 * new Vector3(Mathf.Cos(offsetAngle), 0, Mathf.Sin(offsetAngle));
        Transform movingLeg = shouldMoveLeft ? leftTarget : rightTarget;
        Vector3 movement = (balanceRadius - center.localPosition.z) * dir + (center.position - movingLeg.position) + offset;

        if (shouldMoveLeft) {
            leftLeaveGroundTime = Time.time;
            nextLeftPosition = new Vector3(leftTarget.position.x, 0, leftTarget.position.z) + movement;
            lastLeftGroundPosition = new Vector3(leftTarget.position.x, 0, leftTarget.position.z);

            StartCoroutine(PlayStepSound(leftFootSound));
        }
        else {
            rightLeaveGroundTime = Time.time;
            nextRightPosition = new Vector3(rightTarget.position.x, 0, rightTarget.position.z) + movement;
            lastRightGroundPosition = new Vector3(rightTarget.position.x, 0, rightTarget.position.z);

            StartCoroutine(PlayStepSound(rightFootSound));
        }
        
        lastBalancePoint = center.position;
    }

    IEnumerator PlayStepSound(AudioSource source) {
        yield return new WaitForSeconds(stepTime);
        float volume = Random.Range(minVolume, maxVolume);
        source.volume = volume;
        source.Play();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastBalancePoint, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.position, balanceRadius);
    }
}
