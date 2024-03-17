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
    public PlayerTargeting playerTargeting;
    public PlayerMovement playerMovement;
    [AllowMutableSyncType]
    public SyncVar<Player> player;


    private void Awake() {
        playerTargeting = GetComponent<PlayerTargeting>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnStartClient() {
        if (!IsOwner)
            return;
        base.OnStartClient();
        SetPlayer(API.instance.clientKey);
        ChatManager.instance.container.SetActive(true);
        PlayerManager.instance.SetPlayer(this);
        UpdatePlayerInformation(API.instance.clientKey);
        ChatManager.instance.playerControls = playerMovement.playerControls;
        
    }

    [ServerRpc(RequireOwnership = true)]
    private void SetPlayer(Key key, NetworkConnection sender = null) {
        player.Value = Database.instance.GetPlayer(key);
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdatePlayerInformation(Key key, NetworkConnection sender = null) {
        Player player = Database.instance.GetPlayer(key);
        UIManager.instance.UpdatePlayerInformation(sender, player);
        UIManager.instance.playerMovement = playerMovement;
        UIManager.instance.playerTargeting = playerTargeting;
    }

    
    private void Update() {
        if (!IsOwner)
            return;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Debug.Log("Lost 10 HP!");
            LoseHealth();
        } else if (Input.GetKeyDown(KeyCode.Equals)) {
            Debug.Log("Lost 10 Mana!");
            LoseMana();
        }
    }

    [ServerRpc]
    private void LoseHealth(NetworkConnection sender = null) {
        player.Value.SetCurrentHealth(player.Value.GetCurrentHealth() - 10);
        Database.instance.UpdatePlayer(player.Value);
    }
    [ServerRpc]
    private void LoseMana(NetworkConnection sender = null) {
        player.Value.SetCurrentMana(player.Value.GetCurrentMana() - 10);
        Database.instance.UpdatePlayer(player.Value);
    }

    public void EnterCombat(NetworkBehaviour target, Player player) {
        ICombatable combatant = target as ICombatable;
        if (!player.GetAggroList().Contains(combatant)) {
            player.GetAggroList().Add(combatant);
            player.SetCombatStatus(CombatStatus.InCombat);

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
        if (player.GetAggroList().Contains(combatant)) {
            player.GetAggroList().Remove(combatant);

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
        var tempTargets = new List<ICombatable>(player.GetAggroList());
        foreach (ICombatable target in tempTargets) {
            ExitCombat(target as NetworkBehaviour, player);
        }
    }

    public Key GetKey() {
        return player.Value.key;
    }


    public Character GetTarget() {
        return player.Value;
    }
    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return TargetType.Player;
    }

    public TargetStatus GetTargetStatus() {
        return player.Value.GetTargetStatus();
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }
}
