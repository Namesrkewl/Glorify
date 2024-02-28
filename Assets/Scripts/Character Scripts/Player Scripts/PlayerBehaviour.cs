using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTargeting))]
public class PlayerBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
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
        PlayerManager.instance.UpdatePlayer(Database.instance.GetPlayer(GetKey()));
    }

    [ObserversRpc]
    private void EnterCombat(NetworkBehaviour target, Player player) {
        ICombatable combatant = target as ICombatable;
        if (!player.aggroList.Contains(combatant)) {
            player.aggroList.Add(combatant);
            player.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            combatant.ServerEnterCombat(this);
        }
    }

    [ServerRpc]
    public void ServerEnterCombat(NetworkBehaviour target) {
        Player player = Database.instance.GetPlayer(GetKey());
        EnterCombat(target, player);

    }

    [ObserversRpc]
    public void ExitCombat(NetworkBehaviour target, Player player) {
        ICombatable combatant = target as ICombatable;
        if (player.aggroList.Contains(combatant)) {
            player.aggroList.Remove(combatant);

            // Notify the other character to exit combat
            combatant.ServerExitCombat(this);
        }
    }

    [ServerRpc]
    public void ServerExitCombat(NetworkBehaviour target) {
        Player player = Database.instance.GetPlayer(GetKey());
        ExitCombat(target, player);
    }

    public Key GetKey() {
        return Client.instance.GetKey();
    }

    public Character GetTarget() {
        return Database.instance.GetPlayer(GetKey());
    }
    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return Database.instance.GetPlayer(GetKey()).targetType;
    }

    public TargetStatus GetTargetStatus() {
        return Database.instance.GetPlayer(GetKey()).targetStatus;
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
                int damage = Random.Range(player.minAutoAttackDamage, player.maxAutoAttackDamage);
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
