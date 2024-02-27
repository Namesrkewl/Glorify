using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // For handling UI input events
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;

public class ActionManager : NetworkBehaviour
{
    //private bool isSwapping = false;
    protected ActionButton firstButtonSelectedForSwap;
    private UIManager uiManager;
    public InputActionMap actionBar;

    private void Awake() {
        uiManager = FindObjectOfType<UIManager>();
    }

    protected virtual void Update()
    {
        if (base.IsClientInitialized)
            HandleInput();
    }

    protected virtual void HandleInput() {
        if (uiManager.ui.FindAction("Click").WasPressedThisFrame() && EventSystem.current.currentSelectedGameObject != null) {
            ActionButton button = EventSystem.current.currentSelectedGameObject.GetComponent<ActionButton>();
            if (button != null) {
                Debug.Log(button);
                if (actionBar.FindAction("Swap Actions").IsPressed()) {
                    if (firstButtonSelectedForSwap == null) {
                        firstButtonSelectedForSwap = button;
                        //isSwapping = true;
                    } else {
                        HandleSwap(button);
                    }
                } else {
                    //isSwapping = false;
                    firstButtonSelectedForSwap = null;
                    button.ExecuteAssignedAction();
                }
            }
        }
    }

    protected virtual void HandleSwap(ActionButton secondButton) {
        firstButtonSelectedForSwap = null;
        //isSwapping = false;
    }
}
