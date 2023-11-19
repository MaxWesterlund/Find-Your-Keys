using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brewer : Usable {
    [SerializeField] GameObject fullPot;
    [SerializeField] GameObject pot;
    bool hasPot;
    bool hasCoffe;

    public override bool CanUse(GameObject item) {
        if (item == null) {
            return false;
        }
        else if (item.GetComponent<Coffe>() == null && item.GetComponent<EmptyPot>() == null) {
            return false;
        }
        else if (hasCoffe && item.GetComponent<Coffe>() != null) {
            return false;
        }
        else if (hasPot && item.GetComponent<EmptyPot>() != null) {
            return false;
        }
        return true;
    }

    public override void Use(GameObject item) {
        if (item.GetComponent<Coffe>() != null) {
            hasCoffe = true;
            if (hasPot) {
                FillPot();
            }
        }
        else if (item.GetComponent<EmptyPot>() != null) {
            hasPot = true;
            pot.SetActive(true);
            if (hasCoffe) {
                FillPot();
            }
        }
    }

    public void FillPot() {
        pot.SetActive(false);
        Instantiate(fullPot, transform.TransformPoint(0, 0.06f, 0), transform.rotation);
    }
}
