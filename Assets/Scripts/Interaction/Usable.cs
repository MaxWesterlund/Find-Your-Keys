using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : Interactable {
    [SerializeField] List<GameObject> requiredItems = new();

    public bool CanUse(GameObject item) {
        if (requiredItems.Count == 0) {
            return true;
        }
        else if (requiredItems.Contains(item)) {
            return true;
        }
        return false;
    }

    public void Use() {
        print("use!");
    }
}
