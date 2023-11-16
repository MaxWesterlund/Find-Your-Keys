using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {
    [SerializeField] Transform hand;
    
    [SerializeField] float reach;

    Interactable highlightedItem;
    Interactable heldItem;

    void Start() {
        PlayerInputManager.Instance.SubPerformed("Interact", OnInteract);
        PlayerInputManager.Instance.SubPerformed("Drop", OnDrop);
    }

    void Update() {
        print(heldItem);
        List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
        if (heldItem != null) {
            interactables.Remove(heldItem);
        }

        if (interactables.Count == 0) {
            highlightedItem = null;
            return;
        }
        
        Interactable closest = interactables[0];
        float closestDistance = Vector3.Distance(hand.position, closest.transform.position);
        foreach (Interactable interactable in interactables) {
            if (interactable == heldItem || interactable == closest) continue;

            float distance = Vector3.Distance(hand.position, interactable.transform.position);
            if (distance < closestDistance) {
                closest = interactable;
                closestDistance = distance;
            }
        }
        
        if (closestDistance <= reach) {
            highlightedItem = closest;
        }
        else {
            highlightedItem = null;
        }
    }

    void OnInteract(InputAction.CallbackContext ctx) {
        if (highlightedItem != null) {
            AddInteractableToHand(highlightedItem);
        }
    }

    void OnDrop(InputAction.CallbackContext ctx) {
        DropHeldItem();
    }

    void AddInteractableToHand(Interactable interactable) {
        if (heldItem != null) {
            heldItem.transform.parent = null;
        }
        heldItem = interactable;
        interactable.transform.parent = hand;
        interactable.transform.localPosition = Vector3.zero;
    }

    void DropHeldItem() {
        if (heldItem == null) return;

        heldItem.transform.parent = null;
        heldItem = null;
    }
}
