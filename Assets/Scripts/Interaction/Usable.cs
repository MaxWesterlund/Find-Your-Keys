using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : Interactable {
    public virtual bool CanUse(GameObject item) {
        if (item == null) {
            return true;
        }
        else {
            return false;
        }
    }

    public virtual void Use(GameObject item) {
        print(this.name + " was used but lack override.");
    }
}
