using UnityEngine;
using UnityEngine.AI;
using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DeathShatter))]
public class NPCBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
    #region Variables
    protected DeathShatter deathShatter;
    public CombatManager combatManager;
    public float moveSpeed = 7.1f;
    public float maxAttackRange = 50f;
    private float autoAttackTimer = 0f;
    private NavMeshAgent agent;
    public float aggroRange = 10f; // Base range for initiating combat
    private Vector3 lastAttackPosition; // Last position where the NPC attacked the player
    private Vector3 originalPosition;
    private TargetType initialStatus;
    private int ID;
    public NPC npc;
    #endregion

    public override void OnStartServer() {
        if (!base.IsServerInitialized)
            return;

        deathShatter = GetComponent<DeathShatter>();
        combatManager = FindObjectOfType<CombatManager>();
        agent = GetComponent<NavMeshAgent>();

        SetStartingValues();
        originalPosition = transform.position;

        // Initialize based on npc
        agent.speed = moveSpeed;

        // Adjust NavMeshAgent settings for NPC collision
        agent.radius = 0.5f; // Adjust as needed for the size of your NPCs
        agent.height = 1.8f; // Adjust based on NPC height

        // Set the obstacle avoidance type to none or low
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        //npc = Database.instance.GetNPC(ID);
    }

    private void Update() {
        if (!base.IsServerInitialized)
            return;
        if (npc.currentHealth <= 0 && (npc.targetStatus != TargetStatus.Dead && npc.combatStatus != CombatStatus.Resetting)) {
            Death();
        } else if (npc.targetStatus != TargetStatus.Dead && npc.combatStatus != CombatStatus.Resetting) {
            if (npc.currentHealth > npc.maxHealth) {
                npc.currentHealth = npc.maxHealth;
            } else if (npc.currentHealth < 0) {
                npc.currentHealth = 0;
            }
        } else {
            return;
        }
        CheckForEnemiesInRange();
        HandleCombatState();
    }

    public void SetStartingValues() {
        Debug.Log("Hi Nerd");
        //npc.currentHealth = npc.maxHealth;
        //npc.currentMana = npc.maxMana;
    }

    #region Movement Logic
    private void PathToTarget(GameObject target) {
        agent.SetDestination(target.transform.position);
    }

    private IEnumerator ResetPosition() {
        npc.combatStatus = CombatStatus.Resetting;

        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(npc.aggroList);

        foreach (var character in tempCharacters) {
            ExitCombat(character);
        }
        agent.enabled = false;

        while (Vector3.Distance(transform.position, originalPosition) > 0f) {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetStartingValues();
        agent.enabled = true;
        npc.combatStatus = CombatStatus.OutOfCombat;
    }
    #endregion
    #region Combat Logic
    private void HandleCombatState() {
        if (npc.combatStatus == CombatStatus.OutOfCombat) {
            originalPosition = transform.position;
        }
        if (npc.targetStatus == TargetStatus.Dead && npc.currentHealth > 0) {
            StartCoroutine(ResetPosition());
         } else if (npc.targetType == TargetType.Hostile && npc.combatStatus != CombatStatus.Resetting && npc.targetStatus != TargetStatus.Dead) {
            StartCoroutine(InCombat());
        }
    }

    private void CheckForEnemiesInRange() {
        // Logic to detect players within aggro range and add them to combat list
        if (npc.currentHealth > 0 && npc.combatStatus != CombatStatus.Resetting) {
            foreach (var player in FindObjectsOfType<PlayerBehaviour>()) {
                /*
                if (player.playerData.Value) {
                    if (Vector3.Distance(transform.position, player.transform.position) <= CalculateCombatRange(player) && IsLineOfSightClear(player.gameObject) && player.state != State.Dead) {
                        EnterCombat(player);
                    }
                }
                */
            }
        }
    }

    private IEnumerator InCombat() {
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(npc.aggroList);
        foreach (var character in tempCharacters) {
            if (ShouldStopCombat(character)) {
                ExitCombat(character);
            }
        }
        if (npc.aggroList.Count == 0) {
            StartCoroutine(ResetPosition());
        } else {
            npc.combatStatus = CombatStatus.InCombat;
            //Debug.Log("Attacking");
            var target = npc.aggroList[0];
            HandleAutoAttack(target);
            // Look toward the target logic goes here
            if (target != null) {
                GameObject targetObject = target.GetTargetObject();
                Vector3 directionToTarget = targetObject.transform.position - targetObject.transform.position;
                directionToTarget.y = 0; // This ensures the rotation only happens along the y-axis
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                // Apply the rotation to the gameObject
                transform.rotation = targetRotation;
                if (IsTargetInAttackRange(targetObject) && IsLineOfSightClear(targetObject)) {
                    agent.SetDestination(transform.position);
                } else {
                    PathToTarget(targetObject);
                }
            }
            
            yield return null;
        }
    }

    private float CalculateCombatRange(ICombatable _target) {
        Identifiable target = Database.instance.GetTarget(_target);
        
        if (target as Player != null) {
            Player targetAsPlayer = target as Player;
            int levelDifference = Mathf.Abs(targetAsPlayer.level - npc.level);
            if ( targetAsPlayer.level >= npc.level) {
                return aggroRange - Mathf.Min(levelDifference, 5);
            } else {
                return aggroRange + Mathf.Min(levelDifference, 5);
            }
        } else {
            NPC targetAsNPC = target as NPC;
            int levelDifference = Mathf.Abs(targetAsNPC.level - npc.level);
            if (targetAsNPC.level >= npc.level) {
                return aggroRange - Mathf.Min(levelDifference, 5);
            } else {
                return aggroRange + Mathf.Min(levelDifference, 5);
            }
        }

        
    }
    private bool IsTargetInAttackRange(GameObject target) {
        return Vector3.Distance(transform.position, target.transform.position) <= npc.autoAttackRange - 2;
    }

    private bool IsLineOfSightClear(GameObject target) {
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.position;
        float range = (npc.combatStatus == CombatStatus.InCombat) ? npc.autoAttackRange : aggroRange;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, range)) {
            return hit.collider.gameObject == target;
        }
        return false;
    }

    #endregion

    #region Auto Attack Logic

    private void HandleAutoAttack(ICombatable target) {
        if (IsTargetInRangeAndVisible(target)) {
            if (autoAttackTimer <= 0f) {
                Debug.Log("Attacked");
                PerformAutoAttack(target);
                autoAttackTimer = CalculateAutoAttackCooldown();
            } else {
                autoAttackTimer -= Time.deltaTime;
            }
        } else {
            autoAttackTimer -= Time.deltaTime;
        }
    }

    private void PerformAutoAttack(ICombatable _target) {
        Identifiable target = Database.instance.GetTarget(_target);
       
        if (target as Player != null) {
            Player targetAsPlayer = target as Player;
            if (target != null && targetAsPlayer.currentHealth > 0) {
                // Auto-attack logic here...
                Debug.Log("Performed");
                int damage = UnityEngine.Random.Range(npc.autoAttackDamageMin, npc.autoAttackDamageMax);
                combatManager.SendDamage(_target as IDamageable, damage);
            }
        } else {
            NPC targetAsNPC = target as NPC;
            if (target != null && targetAsNPC.currentHealth > 0) {
                // Auto-attack logic here...
                Debug.Log("Performed");
                int damage = UnityEngine.Random.Range(npc.autoAttackDamageMin, npc.autoAttackDamageMax);
                combatManager.SendDamage(_target as IDamageable, damage);
            }
        }
    }

    private bool IsTargetInRangeAndVisible(ICombatable target) {
        if (target == null) return false;

        GameObject targetObject = target.GetTargetObject();

        float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
        if (distanceToTarget > npc.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (targetObject.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, npc.autoAttackRange)) {
            return hit.collider.gameObject == targetObject;
        }

        return false;
    }

    private float CalculateAutoAttackCooldown() {
        return Mathf.Max(npc.autoAttackCooldown, 0.5f);
    }

    #endregion

    #region Combat Logic
    // New method to determine whether to stop combat
    private bool ShouldStopCombat(ICombatable target) {
        return Vector3.Distance(originalPosition, target.GetTargetObject().transform.position) > maxAttackRange;
    }

    // Overriding methods...

    public void EnterCombat(ICombatable target) {
        if (!npc.aggroList.Contains(target)) {
            npc.aggroList.Add(target);
            npc.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            target.EnterCombat(this);
        }
    }

    public void ExitCombat(ICombatable target) {
        if (npc.aggroList.Contains(target)) {
            npc.aggroList.Remove(target);

            // Notify the other character to exit combat
            target.ExitCombat(this);
        }
    }

    #endregion

    #region Death Logic
    private void Death() {
        Debug.Log("Running Death");
        npc.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(npc.aggroList);
        //PlayerBehaviour tempPlayer = null;

        foreach (var character in tempCharacters) {
            /*tempPlayer = character as PlayerBehaviour;
            if (tempPlayer != null) {
                tempPlayer.playerData.Value.currentExperience += npc.experience;
            }*/
            ExitCombat(character);
        }
        // Stop all coroutines to cease current behaviour
        StopAllCoroutines();

        // Disable the NavMeshAgent to stop movement
        if (agent != null) {
            agent.enabled = false;
        }

        if (deathShatter) {
            deathShatter.ShatterCharacter();
        }

        // Additional death behaviour (e.g., playing death animation, dropping items, etc.)
        // ...

        // Optionally disable the GameObject or components to simulate death
        // gameObject.SetActive(false);
        // Or disable relevant components, e.g., colliders, renderers, etc.
    }
    #endregion

    #region Interfaces
    public int GetID() {
        return 0;
    }

    public Identifiable GetTarget() {
        return Database.instance.GetNPC(GetID());
    }

    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return Database.instance.GetNPC(ID).targetType;
    }

    public TargetStatus GetTargetStatus() {
        return TargetStatus.Alive;
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }

    private void OnApplicationQuit() {
        npc.currentHealth = npc.maxHealth;
        npc.currentMana = npc.maxMana;
    }
    #endregion
}