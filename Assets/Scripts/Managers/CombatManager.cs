using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class CombatManager : NetworkBehaviour {

    public static CombatManager instance;

    #region Variables
    FloatingTextManager floatingTextManager;


    #endregion

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            floatingTextManager = FindObjectOfType<FloatingTextManager>();
        }
    }

    public void SendDamage(IDamageable _target, int damage) {
        Identifiable target = Database.instance.GetTarget(_target as ITargetable);
        EntityUI targetUI = floatingTextManager.GetEntityUI(_target as ITargetable);
        if (target as Player != null) {
            Player targetAsPlayer = target as Player;
            targetAsPlayer.currentHealth -= damage;
            if (damage > 0) {
                targetAsPlayer.currentHealth = Mathf.Max(targetAsPlayer.currentHealth, 0);
            } else {
                targetAsPlayer.currentHealth = Mathf.Min(targetAsPlayer.currentHealth, targetAsPlayer.maxHealth);
            }
            floatingTextManager.CreateCombatText(targetUI, (damage * -1).ToString());
        } else {
            NPC targetAsNPC = target as NPC;
            targetAsNPC.currentHealth -= damage;
            if (damage > 0) {
                targetAsNPC.currentHealth = Mathf.Max(targetAsNPC.currentHealth, 0);
            } else {
                targetAsNPC.currentHealth = Mathf.Min(targetAsNPC.currentHealth, targetAsNPC.maxHealth);
            }
            floatingTextManager.CreateCombatText(targetUI, (damage * -1).ToString());
        }
        
    }
}
