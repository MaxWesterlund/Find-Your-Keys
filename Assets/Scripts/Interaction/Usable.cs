using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : Interactable {
    public virtual bool CanUse(GameObject item) {
        return true;
    }

    public virtual void Use(GameObject item) {
        print("use!");
    }
}
