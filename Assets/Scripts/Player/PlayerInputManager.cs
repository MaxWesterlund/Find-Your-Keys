using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : Singleton<PlayerInputManager> {
    [SerializeField] InputActionAsset asset;

    void OnEnable() {
        asset.Enable();
    }

    void OnDisable() {
        asset.Disable();
    }

    public void SubPerformedAndCanceled(string name, Action<InputAction.CallbackContext> s) {
        InputAction action = asset.FindAction(name);
        action.performed += s;
        action.canceled += s;
    }

    public void SubPerformed(string name, Action<InputAction.CallbackContext> s) {
        InputAction action = asset.FindAction(name);
        action.performed += s;
    }
}