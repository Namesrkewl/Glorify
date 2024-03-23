using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

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

    public void Damage(Character receiver, int damage, Character sender = null) {
        receiver.currentHealth -= damage;
        if (damage > 0) {
            receiver.currentHealth = Mathf.Max(receiver.currentHealth, 0);
        } else {
            receiver.currentHealth = Mathf.Min(receiver.currentHealth, receiver.maxHealth);
        }
        receiver.Sync();

        if (receiver as Player != null) {
            SendDamage(receiver.networkObject.Owner, receiver, damage);
        }
        
        if (sender as Player != null) {
            SendDamage(sender.networkObject.Owner, receiver, damage);
        }
    }

    [TargetRpc]
    private void SendDamage(NetworkConnection receiver, Character character, int damage) {
        string _damage = (damage * -1).ToString();
        CombatText.CreateDamageText(character, _damage);
    }

}
