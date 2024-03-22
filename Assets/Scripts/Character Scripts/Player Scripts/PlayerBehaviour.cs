using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameKit.Dependencies.Utilities;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTargeting))]
[RequireComponent(typeof(MeshShatter))]
public class PlayerBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
    public static PlayerBehaviour instance = null;
    public MeshShatter meshShatter;
    [AllowMutableSyncType] public SyncVar<Player> player;
    public readonly SyncVar<bool> isReady = new SyncVar<bool>(false);
    public InformationText informationText;
    public GameObject nameplate;

    private void Awake() {
        meshShatter = GetComponent<MeshShatter>();
        player.OnChange += player_OnChange;
    }

    private void player_OnChange(Player oldPlayer, Player newPlayer, bool asServer) {
        if (!IsOwner) {
            return;
        }
        if (asServer) {
            if (!isReady.Value) {
                isReady.Value = true;
            }
        } else {
            SetReady();
        }
    }

    [ServerRpc]
    private void SetReady() {
        if (!isReady.Value) {
            isReady.Value = true;
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        if (!IsOwner) {
            Sync();
            return;
        }
        instance = this;
        if (Client.instance.mainMenu.activeSelf) {
            Client.instance.mainMenu.SetActive(false);
            Debug.Log("Disabled the main menu!");
        }
        SetPlayer(API.instance.clientKey);
        ChatManager.instance.container.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Sync() {
        player.Value.Sync();
    }

    [ServerRpc]
    private void SetPlayer(Key key, NetworkConnection sender = null) {
        player.Value = Database.instance.GetPlayer(key);
        player.Value.networkObject = NetworkObject;
        player.Value.currentTarget = null;
        player.Value.playerBehaviour = this;
        player.Value.aggroList.Clear();
        player.Value.Sync();
    }


    private void Update() {
        if (player == null || player.Value == null || gameObject.IsDestroyed() || !isReady.Value) return;

        if (IsServerInitialized) {
            UpdatePlayer();
        }

        if (IsClientInitialized) {
            if (informationText == null && player.Value.networkObject != null) {
                InformationText.CreateInformationText(player.Value);
            }
        }

        if (!IsOwner) return;

        HandleAutoAttack(PlayerTargeting.instance.currentTarget);

        UIManager.instance.UpdatePlayerInformation(player.Value);

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Debug.Log("Lost 10 HP!");
            LoseHealth();
        } else if (Input.GetKeyDown(KeyCode.Equals)) {
            Debug.Log("Lost 10 Mana!");
            LoseMana();
        }
    }

    #region Client Side

    [ServerRpc]
    private void LoseHealth(NetworkConnection sender = null) {
        player.Value.currentHealth -= 10;
        CombatManager.instance.SendDamage(Owner, player.Value, -10);
        player.Value.Sync();
    }
    [ServerRpc]
    private void LoseMana(NetworkConnection sender = null) {
        player.Value.currentMana -= 10;
        player.Value.Sync();
    }

    public void EnterCombat(GameObject target) {
        if (!player.Value.aggroList.Contains(target)) {
            player.Value.aggroList.Add(target);
            player.Value.combatStatus = CombatStatus.InCombat;
            player.Value.Sync();
            Debug.Log("Player Entering Combat");

            // Notify the other character to enter combat
            if (!target.IsDestroyed()) target.GetComponent<ICombatable>().ServerEnterCombat(gameObject);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerEnterCombat(GameObject target) {
        EnterCombat(target);
    }

    public void ExitCombat(GameObject target) {
        if (player.Value.aggroList.Contains(target)) {
            player.Value.aggroList.Remove(target);
            player.Value.Sync();
            Debug.Log("Player Exiting Combat");
            // Notify the other character to exit combat
            if (!target.IsDestroyed()) target.GetComponent<ICombatable>().ServerExitCombat(gameObject);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerExitCombat(GameObject target) {
        ExitCombat(target);
    }

    [ServerRpc]
    public void ExitAllCombat() {
        var tempTargets = new List<GameObject>(player.Value.aggroList);
        foreach (GameObject target in tempTargets) {
            ExitCombat(target);
        }
    }

    public Key GetKey() {
        return player.Value.GetKey();
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
        return player.Value.targetStatus;
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }
    #endregion

    #region Server Side

    [Server(Logging = LoggingType.Off)]
    public void UpdatePlayer() {
        if (player.Value.targetStatus == TargetStatus.Dead) {
            return;
        }
        if (player.Value.currentHealth <= 0) {
            Death();
            return;
        }
        UpdateCombatState();
        HandleRegeneration();
        LevelUp();

        /*
        if (player.Value.aggroList.Count > 0) {
            //Debug.Log($"Enemies in combat with the player: {player.Value.aggroList.Count}");
        }*/


        /*
        if (player.Value.playerStatus == Player.PlayerStatus.Dead) {
            playerMovement.enabled = false;
        } else {
            playerMovement.enabled = true;
        }
        */
    }

    [Server(Logging = LoggingType.Off)]
    private void LevelUp() {
        if (player.Value.currentExperience >= player.Value.maxExperience) {
            player.Value.currentExperience -= player.Value.maxExperience;
            player.Value.level += 1;
            //player.Value.playerClass.SetClass(this); // Update class on GetLevel() up
            player.Value.currentHealth = player.Value.maxHealth;
            player.Value.currentMana = player.Value.maxMana;
            player.Value.Sync();
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ChangeClass(Key key, Classes newClass) {
        player.Value.classEnum = newClass;
        //player.Value.playerClass.SetClass(this); // Update class on class change
    }

    #region Resource Generation Logic
    private void HandleRegeneration() {
        HandleHealthRegeneration();
        HandleManaRegeneration();
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleHealthRegeneration() {
        if (player.Value.targetStatus != TargetStatus.Dead && player.Value.currentHealth < player.Value.maxHealth) {
            if (player.Value.isSafe) {
                // Out of combat regeneration
                float healthRegenPerSecond = GetHealthRegeneration() / 10f; // Amount regenerated per second
                player.Value.currentHealth = Mathf.Min(player.Value.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.Value.maxHealth);
            } else {
                // In combat regeneration
                float healthRegenPerSecond = GetCombatHealthRegeneration() / 10f; // Amount regenerated per second
                player.Value.currentHealth = Mathf.Min(player.Value.currentHealth + (healthRegenPerSecond * Time.deltaTime), player.Value.maxHealth);
            }
            player.Value.Sync();
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void HandleManaRegeneration() {
        if (player.Value.currentMana < player.Value.maxMana) {
            if (player.Value.isSafe) {
                // Out of combat regeneration
                float manaRegenPerSecond = GetManaRegeneration() / 10f; // Amount regenerated per second
                player.Value.currentMana = Mathf.Min(player.Value.currentMana + (manaRegenPerSecond * Time.deltaTime), player.Value.maxMana);
            } else {
                // In combat regeneration
                float manaRegenPerSecond = GetCombatManaRegeneration() / 10f; // Amount regenerated per second
                player.Value.currentMana = Mathf.Min(player.Value.currentMana + (manaRegenPerSecond * Time.deltaTime), player.Value.maxMana);
            }
            player.Value.Sync();
        }
    }
    #endregion

    #region Derived Stats
    [Server(Logging = LoggingType.Off)]
    public int GetMeleeAttackPower() {
        return player.Value.strength + player.Value.dexterity; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetRangedAttackPower() {
        return player.Value.dexterity; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetSpellPower() {
        return player.Value.intelligence; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetAttackSpeed() {
        return 1.0f / player.Value.haste; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetCastingSpeed() {
        return 1.0f - (player.Value.haste * 0.01f); // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetHealthRegeneration() {
        return player.Value.vitality * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatHealthRegeneration() {
        return (int)(GetHealthRegeneration() * 0f); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public int GetManaRegeneration() {
        return player.Value.wisdom * 2; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public int GetCombatManaRegeneration() {
        return Mathf.Max((int)(GetManaRegeneration() * 0.1f), 1); // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetGlobalCooldown() {
        return 1.5f / player.Value.haste; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetBlockReductionRate() {
        return 0.25f; // Default value
    }

    [Server(Logging = LoggingType.Off)]
    public float GetPhysicalDamageReduction() {
        return player.Value.armor * 0.01f; // Example calculation, adjust as needed
    }

    [Server(Logging = LoggingType.Off)]
    public float GetMagicalDamageReduction() {
        return player.Value.intelligence * 0.01f; // Example calculation, adjust as needed
    }
    #endregion

    // Add methods for combat, taking damage, etc. as needed
    [Server(Logging = LoggingType.Off)]
    public void SetClass() {
        //player.Value.playerClass.SetClass(this);
        player.Value.Sync();
    }

    #region Combat Logic

    // This method is called to check if the player should exit combat.

    [Server(Logging = LoggingType.Off)]
    private void UpdateCombatState() {
        if (player.Value.isSafe && player.Value.aggroList.Count == 0) {
            return;
        }

        if (player.Value.aggroList.Count == 0 && player.Value.combatStatus == CombatStatus.InCombat) {
            player.Value.combatStatus = CombatStatus.OutOfCombat;
            player.Value.regenerationCooldownTimer = 0f; // Start regeneration cooldown when combat ends
        } else if (player.Value.aggroList.Count > 0) {
            player.Value.combatStatus = CombatStatus.InCombat;
            player.Value.isSafe = false; // Stop regeneration immediately when in combat
            player.Value.regenerationCooldownTimer = 0f; // Reset timer if in combat
        }

        if (player.Value.combatStatus == CombatStatus.OutOfCombat && !player.Value.isSafe) {
            player.Value.regenerationCooldownTimer += Time.deltaTime;
            if (player.Value.regenerationCooldownTimer >= 5f) { // 5 seconds after leaving combat
                player.Value.isSafe = true;
            }
        }
        player.Value.Sync();
    }
    #endregion

    #region Auto Attack Logic

    [ServerRpc]
    private void HandleAutoAttack(GameObject currentTarget) {
        if (currentTarget != null && player.Value.actionState == ActionState.AutoAttacking && IsTargetInRangeAndVisible(currentTarget) && player.Value.autoAttackTimer <= 0f) {
            PerformAutoAttack(currentTarget);
            player.Value.autoAttackTimer = CalculateAutoAttackCooldown();
            player.Value.Sync();
        } else if (player.Value.autoAttackTimer > 0f) {
            player.Value.autoAttackTimer -= Time.deltaTime;
            player.Value.Sync();
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void PerformAutoAttack(GameObject currentTarget) {
        ServerPerformAutoAttack(currentTarget);
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInRangeAndVisible(GameObject currentTarget) {
        if (currentTarget == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > player.Value.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, player.Value.autoAttackRange)) {
            return hit.collider.gameObject == currentTarget;
        }

        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown() {
        float hasteEffect = 1.0f + (player.Value.haste / 100.0f);
        return Mathf.Max(player.Value.autoAttackCooldown / hasteEffect, 0.5f);
    }

    #region Server RPCs

    [ServerRpc(RequireOwnership = true)]
    private void ServerPerformAutoAttack(GameObject target, NetworkConnection sender = null) {
        if (target != null && !target.IsDestroyed()) {
            if (target.GetComponent<ICombatable>() != null && target.GetComponent<ICombatable>().GetTarget().currentHealth > 0) {
                EnterCombat(target); // Enter combat with the target

                // Auto-attack logic here...
                int damage = UnityEngine.Random.Range(player.Value.minAutoAttackDamage, player.Value.maxAutoAttackDamage);
                CombatManager.instance.SendDamage(target.GetComponent<ICombatable>(), damage);
                ConfirmAutoAttack();
            }
        }
    }

    #endregion

    [ObserversRpc]
    private void ConfirmAutoAttack() {
        Debug.Log($"{name} Auto Attacked!");
    }

    #endregion

    #region Death Logic

    [Server(Logging = LoggingType.Off)]
    private void Death() {
        player.Value.targetStatus = TargetStatus.Dead;
        ExitAllCombat();

        if (meshShatter) {
            meshShatter.ShatterCharacter();
        }
        Sync();
    }
    #endregion

    

    #region Rpcs



    #endregion

    #endregion
}
