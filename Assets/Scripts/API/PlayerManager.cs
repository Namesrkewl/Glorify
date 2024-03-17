using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing.Logging;
using System.Linq;
using UnityEditor;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using GameKit.Dependencies.Utilities;
using FishNet.Demo.AdditiveScenes;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager instance;
    private PlayerBehaviour playerBehaviour;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    #region Variables
    //public Spell currentSpell;
    
    private UIManager uiManager;
    #endregion

    // Update is called once per frame
    
    private void Update() {
        if (IsClientInitialized)
            if (playerBehaviour != null && playerBehaviour.player != null && playerBehaviour.player.Value != null && !gameObject.IsDestroyed()) {
                UpdatePlayer(playerBehaviour.player.Value, playerBehaviour.playerTargeting.currentTarget);
            }
    }

    public void SetPlayer(PlayerBehaviour _playerBehaviour) {
        playerBehaviour = _playerBehaviour;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayer(Player player, GameObject currentTarget) {
        UpdateCombatState(player);
        HandleRegeneration(player);
        LevelUp(player);
        UpdateCurrentTarget(player, currentTarget);
        HandleAutoAttack(player, currentTarget);


        /*
        if (player.GetAggroList().Count > 0) {
            //Debug.Log($"Enemies in combat with the player: {player.GetAggroList().Count}");
        }*/


        /*
        if (player.playerStatus == Player.PlayerStatus.Dead) {
            playerMovement.enabled = false;
        } else {
            playerMovement.enabled = true;
        }
        */
    }

    [Server(Logging = LoggingType.Off)]
    private void LevelUp(Player player) {
        if (player.GetCurrentExperience() >= player.GetMaxExperience()) {
            player.SetCurrentExperience(player.GetCurrentExperience() - player.GetMaxExperience());
            player.SetLevel(player.GetLevel() + 1);
            //player.playerClass.SetClass(this); // Update class on GetLevel() up
            player.SetCurrentHealth(player.GetMaxHealth());
            player.SetCurrentMana(player.GetMaxMana());
            Database.instance.UpdatePlayer(player);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ChangeClass(Key key, Classes newClass) {
        Player player = Database.instance.GetPlayer(key);
        player.SetClassEnum(newClass);
        //player.playerClass.SetClass(this); // Update class on class change
        Database.instance.UpdatePlayer(player);
    }

    #region Resource Generation Logic
    private void HandleRegeneration(Player player) {
        HandleHealthRegeneration(player);
        HandleManaRegeneration(player);
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleHealthRegeneration(Player player) {
        if (player.GetCurrentHealth() > 0 && player.GetCurrentHealth() < player.GetMaxHealth()) {
            if (player.GetIsSafe()) {
                // Out of combat regeneration
                float healthRegenPerSecond = GetHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.SetCurrentHealth(Mathf.Min(player.GetCurrentHealth() + (healthRegenPerSecond * Time.deltaTime), player.GetMaxHealth()));
            } else {
                // In combat regeneration
                float healthRegenPerSecond = GetCombatHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.SetCurrentHealth(Mathf.Min(player.GetCurrentHealth() + (healthRegenPerSecond * Time.deltaTime), player.GetMaxHealth()));
            }
            Database.instance.UpdatePlayer(player);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleManaRegeneration(Player player) {
        if (player.GetCurrentMana() < player.GetMaxMana()) {
            if (player.GetIsSafe()) {
                // Out of combat regeneration
                float manaRegenPerSecond = GetManaRegeneration(player) / 10f; // Amount regenerated per second
                player.SetCurrentMana(Mathf.Min(player.GetCurrentMana() + (manaRegenPerSecond * Time.deltaTime), player.GetMaxMana()));
            } else {
                // In combat regeneration
                float manaRegenPerSecond = GetCombatManaRegeneration(player) / 10f; // Amount regenerated per second
                player.SetCurrentMana(Mathf.Min(player.GetCurrentMana() + (manaRegenPerSecond * Time.deltaTime), player.GetMaxMana()));
            }
            Database.instance.UpdatePlayer(player);
        }
    }
    #endregion

    #region Derived Stats
    [Server(Logging = LoggingType.Off)]
    public int GetMeleeAttackPower(Player player) {
        return player.GetStrength() + player.GetDexterity(); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetRangedAttackPower(Player player) {
        return player.GetDexterity(); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetSpellPower(Player player) {
        return player.GetIntelligence(); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetAttackSpeed(Player player) {
        return 1.0f / player.GetHaste(); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetCastingSpeed(Player player) {
        return 1.0f - (player.GetHaste() * 0.01f); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetHealthRegeneration(Player player) {
        return player.GetVitality() * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatHealthRegeneration(Player player) {
        return (int)(GetHealthRegeneration(player) * 0f); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public int GetManaRegeneration(Player player) {
        return player.GetWisdom() * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatManaRegeneration(Player player) {
        return Mathf.Max((int)(GetManaRegeneration(player) * 0.1f), 1); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetGlobalCooldown(Player player) {
        return 1.5f / player.GetHaste(); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetBlockReductionRate(Player player) {
        return 0.25f; // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetPhysicalDamageReduction(Player player) {
        return player.GetArmor() * 0.01f; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetMagicalDamageReduction(Player player) {
        return player.GetIntelligence() * 0.01f; // Example calculation, adjust as needed
    }
    #endregion

    // Add methods for combat, taking damage, etc. as needed
    [Server(Logging = LoggingType.Off)]
    public void SetClass(Key key) {
        Player player = Database.instance.GetPlayer(key);
        //player.playerClass.SetClass(this);
        Database.instance.UpdatePlayer(player);
    }

    #region Combat Logic

    // This method is called to check if the player should exit combat.

    [Server(Logging = LoggingType.Off)]
    private void UpdateCombatState(Player player) {
        if (player.GetIsSafe() && player.GetAggroList().Count == 0) {
            return;
        }

        if (player.GetAggroList().Count == 0 && player.GetCombatStatus() == CombatStatus.InCombat) {
            player.SetCombatStatus(CombatStatus.OutOfCombat);
            player.SetRegenerationCooldownTimer(0f); // Start regeneration cooldown when combat ends
        } else if (player.GetAggroList().Count > 0) {
            player.SetCombatStatus(CombatStatus.InCombat);
            player.SetIsSafe(false); // Stop regeneration immediately when in combat
            player.SetRegenerationCooldownTimer(0f); // Reset timer if in combat
        }

        if (player.GetCombatStatus() == CombatStatus.OutOfCombat && !player.GetIsSafe()) {
            player.SetRegenerationCooldownTimer(player.GetRegenerationCooldownTimer() + Time.deltaTime);
            if (player.GetRegenerationCooldownTimer() >= 5f) { // 5 seconds after leaving combat
                player.SetIsSafe(true);
            }
        }
        Database.instance.UpdatePlayer(player);
    }
    #endregion

    #region Auto Attack Logic

    [Server(Logging = LoggingType.Off)]
    private void HandleAutoAttack(Player player, GameObject currentTarget) {
        if (player.GetActionState() == ActionState.AutoAttacking && IsTargetInRangeAndVisible(player, currentTarget)) {
            if (player.GetAutoAttackTimer() <= 0f) {
                PerformAutoAttack(player, currentTarget);
                player.SetAutoAttackTimer(CalculateAutoAttackCooldown(player));
            } else {
                player.SetAutoAttackTimer(player.GetAutoAttackTimer() - Time.deltaTime);
            }
        } else {
            player.SetAutoAttackTimer(player.GetAutoAttackTimer() - Time.deltaTime);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void PerformAutoAttack(Player player, GameObject currentTarget) {
        ServerPerformAutoAttack(player, currentTarget);
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInRangeAndVisible(Player player, GameObject currentTarget) {
        if (currentTarget == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > player.GetAutoAttackRange()) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, player.GetAutoAttackRange())) {
            return hit.collider.gameObject == currentTarget;
        }

        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown(Player player) {
        float hasteEffect = 1.0f + (player.GetHaste() / 100.0f);
        return Mathf.Max(player.GetAutoAttackCooldown() / hasteEffect, 0.5f);
    }

    [Server(Logging = LoggingType.Off)]
    private void UpdateCurrentTarget(Player player, GameObject currentTarget) {
        if (player.GetCurrentTarget() != currentTarget) {
            player.SetCurrentTarget(currentTarget);
            Database.instance.UpdatePlayer(player);
        }
    }

    #region Server RPCs

    [ServerRpc(RequireOwnership = true)]
    private void ServerPerformAutoAttack(Player player, GameObject targetObject, NetworkConnection sender = null) {
        if (targetObject != null && !targetObject.IsDestroyed()) {
            ICombatable target = targetObject.GetComponent<ICombatable>();
            if (target != null && target.GetTarget().GetCurrentHealth() > 0) {
                playerBehaviour.EnterCombat(target as NetworkBehaviour, player); // Enter combat with the target

                // Auto-attack logic here...
                int damage = UnityEngine.Random.Range(player.GetMinAutoAttackDamage(), player.GetMaxAutoAttackDamage());
                CombatManager.instance.SendDamage(target, damage);
                ConfirmAutoAttack(player);
            }
        }
    }

    #endregion

    [ObserversRpc]
    private void ConfirmAutoAttack(Player player) {
        Debug.Log($"{name} Auto Attacked!");
    }

    #endregion

    #region Death Logic

    [Server(Logging = LoggingType.Off)]
    private void Death(Key key) {
        Player player = Database.instance.GetPlayer(key);
        player.SetTargetStatus(TargetStatus.Dead);
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(player.GetAggroList());
            
        playerBehaviour.ExitAllCombat();

        /*
        if (player.deathShatter) {
            player.deathShatter.ShatterCharacter();
        }
        */
        Database.instance.UpdatePlayer(player);
    }
    #endregion

    [Server(Logging = LoggingType.Off)]
    public void CreatePlayer(string name, Credentials credentials, NetworkConnection sender) {
        Debug.Log("Creating New Player!");
        Player player = new Player();
        player.SetName(name);
        List<Player> sameNamedPlayers = Database.instance.GetAllPlayers().Where(p => p.GetName() == player.GetName()).ToList();
        List<int> excludedIDs = sameNamedPlayers.Select(p => p.GetID()).ToList();
        do {
            player.SetID(Random.Range(0, 9999999));
        } while (excludedIDs.Contains(player.GetID()));
        Database.instance.AddPlayer(player, credentials);
        Database.instance.AddClient(player, sender);
        Debug.Log("Added client to list");
        Key key = player.key;
        Debug.Log(key.name);
        Debug.Log(player.GetName());
        API.instance.CompleteLogin(sender, key);
    }

    #region Rpcs



    #endregion
}
