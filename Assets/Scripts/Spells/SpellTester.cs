using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTester : NetworkBehaviour {
    public Spell spell;

    private void Update() {
        if (!IsClientInitialized) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && PlayerBehaviour.instance.player.Value != null
            && PlayerTargeting.instance.currentTarget != null && PlayerTargeting.instance.currentTarget.GetComponent<ICombatable>().GetTarget() != null)
            Cast(PlayerBehaviour.instance);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            Debug.Log("Invalid Target.");
    }

    [ServerRpc(RequireOwnership = false)]
    private void Cast(PlayerBehaviour playerBehaviour) {
        spell.Cast(playerBehaviour.player.Value, playerBehaviour.player.Value.currentTarget.GetComponent<ICombatable>().GetTarget());;    
    }
}
