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
            if (playerBehaviour != null && !gameObject.IsDestroyed())
                UpdatePlayer(API.instance.clientKey, playerBehaviour.playerTargeting.currentTarget);
    }

    public void SetPlayer(PlayerBehaviour _playerBehaviour) {
        playerBehaviour = _playerBehaviour;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayer(Key key, GameObject currentTarget) {
        Player player = Database.instance.GetPlayer(key);
        UpdateCombatState(player);
        HandleRegeneration(player);
        LevelUp(player);
        UpdateCurrentTarget(player, currentTarget);
        HandleAutoAttack(player, currentTarget);


        /*
        if (player.aggroList.Count > 0) {
            //Debug.Log($"Enemies in combat with the player: {player.aggroList.Count}");
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
        if (player.currentExperience >= player.maxExperience) {
            player.currentExperience -= player.maxExperience;
            player.level++;
            //player.playerClass.SetClass(this); // Update class on level up
            player.currentHealth = player.maxHealth;
            player.currentMana = player.maxMana;
            Database.instance.UpdatePlayer(player);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ChangeClass(Key key, Classes newClass) {
        Player player = Database.instance.GetPlayer(key);
        player.classEnum = newClass;
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
        if (player.currentHealth > 0 && player.currentHealth < player.maxHealth) {
            if (player.isSafe) {
                // Out of combat regeneration
                float healthRegenPerSecond = GetHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.currentHealth = Mathf.Min(player.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.maxHealth);
            } else {
                // In combat regeneration
                float healthRegenPerSecond = GetCombatHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.currentHealth = Mathf.Min(player.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.maxHealth);
            }
            Database.instance.UpdatePlayer(player);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleManaRegeneration(Player player) {
        if (player.currentMana < player.maxMana) {
            if (player.isSafe) {
                // Out of combat regeneration
                float manaRegenPerSecond = GetManaRegeneration(player) / 10f; // Amount regenerated per second
                player.currentMana = Mathf.Min(player.currentMana + (manaRegenPerSecond * Time.deltaTime), player.maxMana);
            } else {
                // In combat regeneration
                float manaRegenPerSecond = GetCombatManaRegeneration(player) / 10f; // Amount regenerated per second
                player.currentMana = Mathf.Min(player.currentMana + (manaRegenPerSecond * Time.deltaTime), player.maxMana);
            }
            Database.instance.UpdatePlayer(player);
        }
    }
    #endregion

    #region Derived Stats
    [Server(Logging = LoggingType.Off)]
    public int GetMeleeAttackPower(Player player) {
        return player.strength + player.dexterity; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetRangedAttackPower(Player player) {
        return player.dexterity; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetSpellPower(Player player) {
        return player.intelligence; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetAttackSpeed(Player player) {
        return 1.0f / player.haste; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetCastingSpeed(Player player) {
        return 1.0f - (player.haste * 0.01f); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetHealthRegeneration(Player player) {
        return player.vitality * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatHealthRegeneration(Player player) {
        return (int)(GetHealthRegeneration(player) * 0f); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public int GetManaRegeneration(Player player) {
        return player.wisdom * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatManaRegeneration(Player player) {
        return Mathf.Max((int)(GetManaRegeneration(player) * 0.1f), 1); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetGlobalCooldown(Player player) {
        return 1.5f / player.haste; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetBlockReductionRate(Player player) {
        return 0.25f; // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetPhysicalDamageReduction(Player player) {
        return player.armor * 0.01f; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetMagicalDamageReduction(Player player) {
        return player.intelligence * 0.01f; // Example calculation, adjust as needed
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
        if (player.isSafe && player.aggroList.Count == 0) {
            return;
        }

        if (player.aggroList.Count == 0 && player.combatStatus == CombatStatus.InCombat) {
            player.combatStatus = CombatStatus.OutOfCombat;
            player.regenerationCooldownTimer = 0f; // Start regeneration cooldown when combat ends
        } else if (player.aggroList.Count > 0) {
            player.combatStatus = CombatStatus.InCombat;
            player.isSafe = false; // Stop regeneration immediately when in combat
            player.regenerationCooldownTimer = 0f; // Reset timer if in combat
        }

        if (player.combatStatus == CombatStatus.OutOfCombat && !player.isSafe) {
            player.regenerationCooldownTimer += Time.deltaTime;
            if (player.regenerationCooldownTimer >= 5f) { // 5 seconds after leaving combat
                player.isSafe = true;
            }
        }
        Database.instance.UpdatePlayer(player);
    }
    #endregion

    #region Auto Attack Logic

    [Server(Logging = LoggingType.Off)]
    private void HandleAutoAttack(Player player, GameObject currentTarget) {
        if (player.actionState == ActionState.AutoAttacking && IsTargetInRangeAndVisible(player, currentTarget)) {
            if (player.autoAttackTimer <= 0f) {
                PerformAutoAttack(player, currentTarget);
                player.autoAttackTimer = CalculateAutoAttackCooldown(player);
            } else {
                player.autoAttackTimer -= Time.deltaTime;
            }
        } else {
            player.autoAttackTimer -= Time.deltaTime;
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
        if (distanceToTarget > player.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, player.autoAttackRange)) {
            return hit.collider.gameObject == currentTarget;
        }

        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown(Player player) {
        float hasteEffect = 1.0f + (player.haste / 100.0f);
        return Mathf.Max(player.autoAttackCooldown / hasteEffect, 0.5f);
    }

    [Server(Logging = LoggingType.Off)]
    private void UpdateCurrentTarget(Player player, GameObject currentTarget) {
        if (player.currentTarget != currentTarget) {
            player.currentTarget = currentTarget;
            Database.instance.UpdatePlayer(player);
        }
    }

    #region Server RPCs

    [ServerRpc(RequireOwnership = true)]
    private void ServerPerformAutoAttack(Player player, GameObject targetObject, NetworkConnection sender = null) {
        if (targetObject != null && !targetObject.IsDestroyed()) {
            ICombatable target = targetObject.GetComponent<ICombatable>();
            if (target != null && target.GetTarget().currentHealth > 0) {
                playerBehaviour.EnterCombat(target as NetworkBehaviour, player); // Enter combat with the target

                // Auto-attack logic here...
                int damage = UnityEngine.Random.Range(player.minAutoAttackDamage, player.maxAutoAttackDamage);
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
        player.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(player.aggroList);
            
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
        player.key.name = name;
        List<Player> sameNamedPlayers = Database.instance.GetAllPlayers().Where(p => p.key.name == player.key.name).ToList();
        List<int> excludedIDs = sameNamedPlayers.Select(p => p.key.ID).ToList();
        do {
            player.key.ID = Random.Range(0, 9999999);
        } while (excludedIDs.Contains(player.key.ID));
        Database.instance.AddPlayer(player, credentials);
        Database.instance.AddClient(player.key, sender);
        Debug.Log("Added client to list");
        API.instance.CompleteLogin(sender, Database.instance.GetPlayer(player.key).key);
    }

    #region Rpcs



    #endregion
}
