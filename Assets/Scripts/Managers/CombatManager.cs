using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using static UnityEngine.GraphicsBuffer;

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

    public void SendDamage(ITargetable targetObject, int damage) {
        Character target = Database.instance.GetTarget(targetObject);
        EntityUI targetUI = floatingTextManager.GetEntityUI(targetObject);
        target.SetCurrentHealth(target.GetCurrentHealth() - damage);
        if (damage > 0) {
            target.SetCurrentHealth(Mathf.Max(target.GetCurrentHealth(), 0));
        } else {
            target.SetCurrentHealth(Mathf.Min(target.GetCurrentHealth(), target.GetMaxHealth()));
        }
        floatingTextManager.CreateCombatText(targetUI, (damage * -1).ToString());
    }
}
