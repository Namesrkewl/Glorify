using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using MoonSharp.Interpreter;
using FishNet.Managing.Logging;

[MoonSharpUserData]
public class CombatManager : NetworkBehaviour {

    public static CombatManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void Damage(Character receiver, int damage, Character sender = null) {
        float health = receiver.currentHealth;
        health -= Mathf.Max(0, damage);
        receiver.currentHealth = Mathf.Max(health, 0);
        receiver.Sync();

        if (receiver is Player) {
            SendDamage(receiver.networkObject.Owner, receiver, (-damage).ToString());
        }
        
        if (sender is Player && sender != receiver) {
            SendDamage(sender.networkObject.Owner, receiver, (-damage).ToString());
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void Heal(Character receiver, int recovery, Character sender = null) {
        float health = receiver.currentHealth;
        health += Mathf.Max(0, recovery);
        receiver.currentHealth = Mathf.Min(health, receiver.maxHealth);
        receiver.Sync();

        if (receiver is Player) {
            SendDamage(receiver.networkObject.Owner, receiver, recovery.ToString());
        }

        if (sender is Player && sender != receiver) {
            SendDamage(sender.networkObject.Owner, receiver, recovery.ToString());
        }
    }

    [TargetRpc]
    private void SendDamage(NetworkConnection receiver, Character character, string damage) {
        CombatText.CreateDamageText(character, damage);
    }

}
