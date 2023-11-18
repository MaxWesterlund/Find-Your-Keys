using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {
    [SerializeField] Transform hand;
    
    [SerializeField] float reach;

    Interactable highlightedInteractable;
    GameObject heldItem;

    void Start() {
        PlayerInputManager.Instance.SubPerformed("Interact", OnInteract);
        PlayerInputManager.Instance.SubPerformed("Drop", OnDrop);
    }

    void Update() {
        List<Interactable> interactables = FindObjectsOfType<Interactable>().ToList();
        if (heldItem != null) {
            interactables.Remove(heldItem.GetComponent<Interactable>());
        }

        if (interactables.Count == 0) {
            highlightedInteractable = null;
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
            highlightedInteractable = closest;
        }
        else {
            highlightedInteractable = null;
            return;
        }
    }

    void OnInteract(InputAction.CallbackContext ctx) {
        if (highlightedInteractable == null) return;
            
        string baseType = highlightedInteractable.GetType().BaseType.Name; 
        if (baseType == "Item") {
            AddItemToHand(highlightedInteractable.gameObject);
        }
        else if (baseType == "Usable") {
            UseUsable(highlightedInteractable.gameObject.GetComponent<Usable>());
        }
    }

    void OnDrop(InputAction.CallbackContext ctx) {
        DropHeldItem();
    }

    void AddItemToHand(GameObject item) {
        if (heldItem != null) {
            heldItem.transform.parent = null;
        }
        heldItem = item;
        item.transform.parent = hand;
        item.transform.localPosition = Vector3.zero;
    }

    void UseUsable(Usable usable) {
        if (usable.CanUse(heldItem)) {
            usable.Use();
            RemoveHeldItem();
        }
    }

    void DropHeldItem() {
        if (heldItem == null) return;

        heldItem.transform.parent = null;
        heldItem = null;
    }

    void RemoveHeldItem() {
        Destroy(heldItem);
        heldItem = null;
    }
}
