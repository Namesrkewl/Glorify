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
        instance = this;
        floatingTextManager = FindObjectOfType<FloatingTextManager>();
    }

    public void SendDamage(EntityBehaviour targetBehavior, int damage) {
        Character target = targetBehavior.characterData;
        EntityUI targetUI = floatingTextManager.GetEntityUI(targetBehavior);
        target.currentHealth -= damage;
        if (damage > 0) {
            target.currentHealth = Mathf.Max(target.currentHealth, 0);
        } else {
            target.currentHealth = Mathf.Min(target.currentHealth, target.maxHealth);
        }
        floatingTextManager.CreateCombatText(targetUI, (damage * -1).ToString());
    }
}
