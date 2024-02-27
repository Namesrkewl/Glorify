using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // For handling UI input events
using UnityEngine.InputSystem;

public class SpellbookManager : ActionManager {
    public static SpellbookManager instance;
    public GameObject spellContainer;
    public List<Spell> spellList;

    public override void OnStartClient() {
        base.OnStartClient();
        instance = this;
    }

    protected override void HandleSwap(ActionButton secondButton) {
        if (firstButtonSelectedForSwap == null || secondButton == null) return;
        if (!(firstButtonSelectedForSwap as SpellbookButton) || !(secondButton as ActionBarButton)) {
            Debug.Log("Exiting Spellbook assignment.");
            base.HandleSwap(secondButton);
        } else {
            secondButton.AssignAction(firstButtonSelectedForSwap.assignedAction);
            Debug.Log("Assigned a spell from Spellbook!");
            base.HandleSwap(secondButton);
        }
    }
}