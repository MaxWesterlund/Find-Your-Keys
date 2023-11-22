using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {
    [SerializeField] Transform leftHandGrip;
    [SerializeField] Transform rightHandGrip;
    [SerializeField] Transform leftTarget;
    [SerializeField] Transform rightTarget;
    [SerializeField] Transform center;

    [SerializeField] Vector3 handRestingPosition;
    [SerializeField] Vector3 handReadyPosition;
    [SerializeField] Vector3 handHoldingPosition;
    [SerializeField] Vector3 handReadyHoldingPosition;

    [SerializeField] float reach;

    [SerializeField] float handMoveSpeed;
    
    Hand leftHand;
    Hand rightHand;
    Hand currentHand;

    Interactable highlightedInteractable;

    bool isReaching;

    void Start() {
        PlayerInputManager.Instance.SubPerformedAndCanceled("Left Reach", OnLeftReach);
        PlayerInputManager.Instance.SubPerformedAndCanceled("Right Reach", OnRightReach);
        PlayerInputManager.Instance.SubPerformed("Interact", OnInteract);
        PlayerInputManager.Instance.SubPerformed("Drop", OnDrop);

        leftHand = new Hand(
            "Left", leftTarget, leftHandGrip
        );
        rightHand = new Hand(
            "Right", rightTarget, rightHandGrip
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
        int handLocation = hand.Name == "Left" ? -1 : 1;
        if (isReaching && hand.Name == currentHand.Name) {
            if (highlightedInteractable != null) {
                targetPos = highlightedInteractable.transform.position;
            }
            else if (hand.HeldItem != null) {
                targetPos = transform.TransformPoint(new Vector3(handReadyHoldingPosition.x * handLocation, handReadyHoldingPosition.y, handHoldingPosition.z));
            }
            else {
                targetPos = transform.TransformPoint(new Vector3(handReadyPosition.x * handLocation, handReadyPosition.y, handReadyPosition.z));
            }
        }
        else if (hand.HeldItem != null) {
            targetPos = transform.TransformPoint(new Vector3(handHoldingPosition.x * handLocation, handHoldingPosition.y, handHoldingPosition.z));
        }
        else {
            targetPos = transform.TransformPoint(new Vector3(handRestingPosition.x * handLocation, handRestingPosition.y, handRestingPosition.z));
        }
        hand.Target.position = Vector3.Lerp(hand.Target.position, targetPos, handMoveSpeed * Time.deltaTime);
    }

    void OnLeftReach(InputAction.CallbackContext ctx) {
        bool value = ctx.ReadValueAsButton();
        isReaching = value;
        currentHand = leftHand;
    }

    void OnRightReach(InputAction.CallbackContext ctx) {
        bool value = ctx.ReadValueAsButton();
        isReaching = value;
        currentHand = rightHand;
    }

    void OnInteract(InputAction.CallbackContext ctx) {
        if (highlightedInteractable == null || !isReaching) return;
            
        string baseType = GetBaseType(highlightedInteractable);
        if (baseType == "Item") {
            AddItemToHand(highlightedInteractable.gameObject);
        }
        else if (baseType == "Usable") {
            UseUsable(highlightedInteractable.gameObject.GetComponent<Usable>());
        }
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
        Transform hold = item.transform.Find("Hold");
        item.transform.parent = currentHand.Grip;
        item.transform.localRotation = hold.localRotation;
        item.transform.localPosition = currentHand.Grip.InverseTransformPoint(currentHand.Grip.position + item.transform.position - hold.position);
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

        public GameObject HeldItem;

        public Hand(string name, Transform target, Transform grip) {
            Name = name;
            Target = target;
            Grip = grip;
            HeldItem = null;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center.position, reach);
    }
}
