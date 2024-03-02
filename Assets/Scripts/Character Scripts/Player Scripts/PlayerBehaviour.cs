using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameKit.Dependencies.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTargeting))]
[RequireComponent(typeof(MeshShatter))]
public class PlayerBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
    private PlayerTargeting playerTargeting;
    private PlayerMovement playerMovement;
    [AllowMutableSyncType]
    public SyncVar<Key> key;


    private void Awake() {
        playerTargeting = GetComponent<PlayerTargeting>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnStartClient() {
        if (!IsOwner)
            return;
        base.OnStartClient();
        SetKey(API.instance.clientKey);
        ChatManager.instance.container.SetActive(true);
        PlayerManager.instance.SetPlayer(this);
        UpdatePlayerInformation(API.instance.clientKey);
        ChatManager.instance.playerControls = playerMovement.playerControls;
        
    }

    [ServerRpc(RequireOwnership = true)]
    private void SetKey(Key _key, NetworkConnection sender = null) {
        key.Value = _key;
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdatePlayerInformation(Key _key, NetworkConnection sender = null) {
        UIManager.instance.UpdatePlayerInformation(sender, Database.instance.GetPlayer(_key));
    }

    
    private void Update() {
        if (!IsOwner)
            return;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Debug.Log("Lost 10 HP!");
            LoseHealth(key.Value);
        }
    }

    [ServerRpc]
    private void LoseHealth(Key _key, NetworkConnection sender = null) {
        Database.instance.GetPlayer(_key).currentHealth -= 10;
        Database.instance.UpdatePlayer(Database.instance.GetPlayer(_key));
    }

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

    [ServerRpc]
    public void ExitAllCombat() {
        Player player = Database.instance.GetPlayer(GetKey());
        var tempTargets = new List<ICombatable>(player.aggroList);
        foreach (ICombatable target in tempTargets) {
            ExitCombat(target as NetworkBehaviour, player);
        }
    }

    public Key GetKey() {
        return key.Value;
    }


    public Character GetTarget() {
        return Database.instance.GetPlayer(key.Value);
    }
    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return TargetType.Player;
    }

    public TargetStatus GetTargetStatus() {
        return Database.instance.GetPlayer(key.Value).targetStatus;
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
