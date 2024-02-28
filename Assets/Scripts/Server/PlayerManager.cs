using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing.Logging;
using System.Linq;
using UnityEditor;
using FishNet.Object.Synchronizing;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager instance;

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

    [Server(Logging = LoggingType.Off)]
    public void UpdatePlayer(Player player) {
        
        
        UpdateCombatState(player);
        HandleRegeneration(player);

        if (player.currentExperience >= player.maxExperience) {
            LevelUp(player);
        }
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
        player.currentExperience -= player.maxExperience;
        player.level++;
        //player.playerClass.SetClass(this); // Update class on level up
        player.currentHealth = player.maxHealth;
        player.currentMana = player.maxMana;
    }

    [Server(Logging = LoggingType.Off)]
    public void ChangeClass(Player player, PlayerClass.Classes newClass) {
        player.playerClass.playerClass = newClass;
        //player.playerClass.SetClass(this); // Update class on class change
    }

    #region Resource Generation Logic
    private void HandleRegeneration(Player player) {
        HandleHealthRegeneration(player);
        HandleManaRegeneration(player);
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleHealthRegeneration(Player player) {
        if (player.currentHealth > 0) {
            if (player.isSafe) {
                // Out of combat regeneration
                float healthRegenPerSecond = GetHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.currentHealth = Mathf.Min(player.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.maxHealth);
            } else {
                // In combat regeneration
                float healthRegenPerSecond = GetCombatHealthRegeneration(player) / 10f; // Amount regenerated per second
                player.currentHealth = Mathf.Min(player.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.maxHealth);
            }
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleManaRegeneration(Player player) {
        if (player.isSafe) {
            // Out of combat regeneration
            float manaRegenPerSecond = GetManaRegeneration(player) / 10f; // Amount regenerated per second
            player.currentMana = Mathf.Min(player.currentMana + (manaRegenPerSecond * Time.deltaTime), player.maxMana);
        } else {
            // In combat regeneration
            float manaRegenPerSecond = GetCombatManaRegeneration(player) / 10f; // Amount regenerated per second
            player.currentMana = Mathf.Min(player.currentMana + (manaRegenPerSecond * Time.deltaTime), player.maxMana);
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
    public void SetClass(Player player) {
        //player.playerClass.SetClass(this);
    }

    #region Combat Logic

    [Server(Logging = LoggingType.Off)]
    public void EnterCombatWith(Player player, ICombatable receiver) {
        // Additional NPC or Player specific logic for entering combat
    }

    [Server(Logging = LoggingType.Off)]
    public void ExitCombatWith(Player player, ICombatable receiver) {
        // Additional NPC or Player specific logic for exiting combat
    }
    // This method is called to check if the player should exit combat.

    [Server(Logging = LoggingType.Off)]
    private void UpdateCombatState(Player player) {
        /*
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
        */
    }
    #endregion

    #region Death Logic

    [Server(Logging = LoggingType.Off)]
    private void Death(Player player) {
        player.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(player.aggroList);

        foreach (var character in tempCharacters) {
            ExitCombatWith(player, character);
        }

        /*
        if (player.deathShatter) {
            player.deathShatter.ShatterCharacter();
        }
        */
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
        API.instance.CompleteLogin(sender, Database.instance.GetPlayer(player.key).key);
    }

    #region Rpcs



    #endregion
}
