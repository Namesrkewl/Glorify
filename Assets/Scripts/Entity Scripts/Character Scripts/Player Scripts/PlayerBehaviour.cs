using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTargeting))]
public class PlayerBehaviour : NetworkBehaviour, ICombatable, IDamageable, ICastable, IAbleToAttack, IAbleToCast {
    private PlayerTargeting playerTargeting;
    private PlayerMovement playerMovement;

    private void Awake() {
        playerTargeting = GetComponent<PlayerTargeting>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() {
        //UpdatePlayer();
    }

    private void UpdatePlayer() {
        PlayerManager.instance.UpdatePlayer(Database.instance.GetPlayer(GetID()));
    }

    //[ServerRpc]
    public void EnterCombat(ICombatable target) {
        Player player = Database.instance.GetPlayer(GetID());
        if (!player.aggroList.Contains(target)) {
            player.aggroList.Add(target);
            player.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            target.EnterCombat(this);
        }
    }

    public void ExitCombat(ICombatable target) {
        Player player = Database.instance.GetPlayer(GetID());
        if (player.aggroList.Contains(target)) {
            player.aggroList.Remove(target);

            // Notify the other character to exit combat
            target.ExitCombat(this);
        }
    }

    public int GetID() {
        return Client.instance.GetID();
    }

    public Identifiable GetTarget() {
        return Database.instance.GetPlayer(GetID());
    }
    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return Database.instance.GetPlayer(GetID()).targetType;
    }

    public TargetStatus GetTargetStatus() {
        return TargetStatus.Alive;
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }

    /*
    #region Server RPCs

    [ServerRpc(RequireOwnership = false)]
    private void ServerPerformAutoAttack(Player player, GameObject targetObject, NetworkConnection sender = null) {
        if (targetObject != null) {
            EntityBehaviour targetBehavior = targetObject.GetComponent<EntityBehaviour>();
            if (targetBehavior != null && targetBehavior.npc.currentHealth > 0) {
                EnterCombatWith(player, targetBehavior as NPCBehaviour); // Enter combat with the target

                // Auto-attack logic here...
                int damage = Random.Range(player.autoAttackDamageMin, player.autoAttackDamageMax);
                CombatManager.instance.SendDamage(targetBehavior, damage);
                ConfirmAutoAttack(player);
            }
        }
    }

    #endregion

    [ObserversRpc]
    private void ConfirmAutoAttack(Player player) {
        Debug.Log($"{name} Auto Attacked!");
    }
    */
}
