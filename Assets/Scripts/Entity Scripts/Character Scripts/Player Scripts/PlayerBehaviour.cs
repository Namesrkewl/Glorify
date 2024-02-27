using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using FishNet.Demo.AdditiveScenes;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTargeting))]
public class PlayerBehaviour : NetworkBehaviour, ICombatable, IAttackable, ICastable, IAbleToAttack, IAbleToCast {
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
            if (targetBehavior != null && targetBehavior.characterData.currentHealth > 0) {
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
