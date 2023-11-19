using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {
    [SerializeField] Transform leftHandGrip;
    [SerializeField] Transform rightHandGrip;
    [SerializeField] Transform leftTarget;
    [SerializeField] Transform rightTarget;
    [SerializeField] Transform center;

    [SerializeField] Vector3 handRestingPosition;
    [SerializeField] Vector3 handHoldingPosition;

    [SerializeField] float reach;

    [SerializeField] float handMoveSpeed;
    
    Hand leftHand;
    Hand rightHand;
    Hand currentHand;

    Interactable highlightedInteractable;

    bool isReaching;

    void Start() {
        PlayerInputManager.Instance.SubPerformedAndCanceled("Reach", OnReach);
        PlayerInputManager.Instance.SubPerformed("Interact", OnInteract);
        PlayerInputManager.Instance.SubPerformed("Drop", OnDrop);
        PlayerInputManager.Instance.SubPerformed("Left Hand", OnLeftHand);
        PlayerInputManager.Instance.SubPerformed("Right Hand", OnRightHand);

        leftHand = new Hand(
            "Left", leftTarget, leftHandGrip, 
            new Vector3(-handRestingPosition.x, handRestingPosition.y, handRestingPosition.z), 
            new Vector3(-handHoldingPosition.x, handHoldingPosition.y, handHoldingPosition.z)
        );
        rightHand = new Hand(
            "Right", rightTarget, rightHandGrip, handRestingPosition, handHoldingPosition
        );

        currentHand = leftHand;
    }

    void Update() {
        HighlightClosest();
        MoveHand(leftHand);
        MoveHand(rightHand);
    }

    void HighlightClosest() {
        List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
        if (leftHand.HeldItem != null) {
            interactables.Remove(leftHand.HeldItem.GetComponent<Interactable>());
        }
        if (rightHand.HeldItem != null) {
            interactables.Remove(rightHand.HeldItem.GetComponent<Interactable>());
        }

        if (interactables.Count == 0) {
            highlightedInteractable = null;
            return;
        }
        
        Interactable closest = interactables[0];
        float closestDistance = Vector3.Distance(center.position, closest.transform.position);
        foreach (Interactable interactable in interactables) {
            if (interactable == closest) continue;

            float distance = Vector3.Distance(center.position, interactable.transform.position);
            if (distance < closestDistance) {
                closest = interactable;
                closestDistance = distance;
            }
        }
        
        if (closestDistance <= reach) {
            if (GetBaseType(closest) == "Usable") {
                if (!closest.GetComponent<Usable>().CanUse(currentHand.HeldItem)) {
                    return;
                }
            }
            highlightedInteractable = closest;
        }
        else {
            highlightedInteractable = null;
        }
    }

    void MoveHand(Hand hand) {
        Vector3 targetPos;
        if (isReaching && highlightedInteractable != null && hand.Name == currentHand.Name) {
            targetPos = highlightedInteractable.transform.position;
        }
        else if (hand.HeldItem != null) {
            targetPos = transform.TransformPoint(hand.HoldingPosition);
        }
        else {
            targetPos = transform.TransformPoint(hand.RestingPosition);
        }
        hand.Target.position = Vector3.Lerp(hand.Target.position, targetPos, handMoveSpeed * Time.deltaTime);
    }

    void OnReach(InputAction.CallbackContext ctx) {
        bool value = ctx.ReadValueAsButton();
        isReaching = value;
    }

    void OnInteract(InputAction.CallbackContext ctx) {
        if (highlightedInteractable == null) return;
            
        string baseType = GetBaseType(highlightedInteractable);
        if (baseType == "Item") {
            AddItemToHand(highlightedInteractable.gameObject);
        }
        else if (baseType == "Usable") {
            UseUsable(highlightedInteractable.gameObject.GetComponent<Usable>());
        }
    }

    void OnLeftHand(InputAction.CallbackContext ctx) {
        currentHand = leftHand;
    }

    void OnRightHand(InputAction.CallbackContext ctx) {
        currentHand = rightHand;
    }
    
    string GetBaseType(Interactable interactable) {
        return interactable.GetType().BaseType.Name;
    }

    void OnDrop(InputAction.CallbackContext ctx) {
        DropHeldItem();
    }

    void AddItemToHand(GameObject item) {
        if (currentHand.HeldItem != null) {
            currentHand.HeldItem.transform.parent = null;
        }
        currentHand.HeldItem = item;
        item.transform.parent = currentHand.Grip;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localPosition = Vector3.zero;
    }

    void UseUsable(Usable usable) {
        if (usable.CanUse(currentHand.HeldItem)) {
            usable.Use(currentHand.HeldItem);
            RemoveHeldItem();
        }
    }

    void DropHeldItem() {
        if (currentHand.HeldItem == null) return;

        currentHand.HeldItem.transform.parent = null;
        currentHand.HeldItem = null;
    }

    void RemoveHeldItem() {
        Destroy(currentHand.HeldItem);
        currentHand.HeldItem = null;
    }

    class Hand {
        public string Name;
        public Transform Target;
        public Transform Grip;
        public Vector3 RestingPosition;
        public Vector3 HoldingPosition;

        public GameObject HeldItem;

        public Hand(string name, Transform target, Transform grip, Vector3 restingPosition, Vector3 holdingPosition) {
            Name = name;
            Target = target;
            Grip = grip;
            RestingPosition = restingPosition;
            HoldingPosition = holdingPosition;

            HeldItem = null;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center.position, reach);
    }
}
