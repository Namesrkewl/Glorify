using UnityEngine;
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

    public void SendDamage(ITargetable targetObject, int damage) {
        Character target = Database.instance.GetTarget(targetObject);
        EntityUI targetUI = floatingTextManager.GetEntityUI(targetObject);
        target.currentHealth -= damage;
        if (damage > 0) {
            target.currentHealth = Mathf.Max(target.currentHealth, 0);
        } else {
            target.currentHealth = Mathf.Min(target.currentHealth, target.maxHealth);
        }
        floatingTextManager.CreateCombatText(targetUI, (damage * -1).ToString());
    }

}
