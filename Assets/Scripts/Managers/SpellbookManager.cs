using System.Collections.Generic;
using UnityEngine;

public class SpellbookManager : ActionManager {
    public static SpellbookManager instance;
    public GameObject spellContainer;
    public List<Spell> spellList;

    private void Awake() {
        if (instance != null) { 
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();
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