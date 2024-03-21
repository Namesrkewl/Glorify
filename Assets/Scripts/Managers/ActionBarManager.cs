using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionBarManager : ActionManager {
    public List<ActionBarButton> actionBarButtons; // Assign in the Inspector
    public PlayerControls playerControls;

    public override void OnStartClient() {
        base.OnStartClient();
        playerControls = new PlayerControls();
        actionBar = playerControls.ActionBar;
        actionBar.Enable();
    }

    public override void OnStopClient() {
        base.OnStopClient();
        actionBar.Disable();
    }

    protected override void Update() {
        if (base.IsClientInitialized) {
            base.Update();
            UpdateActionBarKeycodes();
        }
    }

    protected override void HandleInput() {
        for (int i = 0; i < actionBarButtons.Count; i++) {
            if (actionBar.FindAction($"Action {i + 1}").triggered) {
                actionBarButtons[i].ExecuteAssignedAction();
            }
        }
        base.HandleInput();
    }

    private void UpdateActionBarKeycodes() {
        for (int i = 0; i < actionBarButtons.Count; i++) {
            actionBarButtons[i].UpdateActionKeycode(actionBar.FindAction($"Action {i + 1}").GetBindingDisplayString());
        }
    }

    protected override void HandleSwap(ActionButton secondButton) {
        if (firstButtonSelectedForSwap == null || secondButton == null) return;
        if (!(firstButtonSelectedForSwap as ActionBarButton) || !(secondButton as ActionBarButton)) {
            Debug.Log("Exiting action bar assignment.");
            base.HandleSwap(secondButton);
        } else {
            PlayerAction temp = firstButtonSelectedForSwap.assignedAction;
            firstButtonSelectedForSwap.AssignAction(secondButton.assignedAction);
            secondButton.AssignAction(temp);
            Debug.Log("Swapped spells on action bar!");
            base.HandleSwap(secondButton);
        }
    }
}