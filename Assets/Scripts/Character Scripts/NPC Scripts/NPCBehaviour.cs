using UnityEngine;
using UnityEngine.AI;
using FishNet.Connection;
using FishNet.Object;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;


//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MeshShatter))]
public class NPCBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
    #region Variables
    protected MeshShatter meshShatter;
    public CombatManager combatManager;
    private NavMeshAgent agent;
    public NPC npc;

    #endregion

    public override void OnStartServer() {
        if (!base.IsServerInitialized)
            return;

        meshShatter = GetComponent<MeshShatter>();
        combatManager = FindObjectOfType<CombatManager>();
        agent = GetComponent<NavMeshAgent>();

        // Initialize based on npc
        agent.speed = npc.speed;

        // Adjust NavMeshAgent settings for NPC collision
        agent.radius = 0.5f; // Adjust as needed for the size of your NPCs
        agent.height = 1.8f; // Adjust based on NPC height

        // Set the obstacle avoidance type to none or low
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        SetStartingValues();
    }

    [Server(Logging = LoggingType.Off)]
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
        }
        HandleCombatState();
    }

    [Server(Logging = LoggingType.Off)]
    public void SetStartingValues() {
        agent.enabled = true;
        npc.currentHealth = npc.maxHealth;
        npc.currentMana = npc.maxMana;
        npc.location = transform.position;
        npc.scale = transform.localScale;
        npc.rotation = transform.rotation;
        npc.targetStatus = TargetStatus.Alive;
        npc.combatStatus = CombatStatus.OutOfCombat;
        npc.actionState = ActionState.Idle;
    }

    [Server(Logging = LoggingType.Off)]
    public void ResetValues() {
        agent.enabled = true;
        npc.currentHealth = npc.maxHealth;
        npc.currentMana = npc.maxMana;
        transform.position = npc.location;
        transform.localScale = npc.scale;
        transform.rotation = npc.rotation;
        npc.targetStatus = TargetStatus.Alive;
        npc.combatStatus = CombatStatus.OutOfCombat;
        npc.actionState = ActionState.Idle;
    }

    #region Movement Logic
    [Server(Logging = LoggingType.Off)]
    private void PathToTarget(GameObject target) {
        agent.SetDestination(target.transform.position);
    }

    [Server(Logging = LoggingType.Off)]
    private IEnumerator ResetPosition() {
        agent.enabled = false;
        npc.combatStatus = CombatStatus.Resetting;

        while (Vector3.Distance(transform.position, npc.location) > 0f) {
            Vector3 direction = npc.location - transform.position;
            direction.y = 0; // This ensures the rotation only happens along the y-axis
            Quaternion targetRotation = Quaternion.identity;
            if (direction != Vector3.zero) {
                targetRotation = Quaternion.LookRotation(direction);
            }
            // Apply the rotation to the gameObject
            transform.rotation = targetRotation;
            transform.position = Vector3.MoveTowards(transform.position, npc.location, npc.speed * Time.deltaTime);
            yield return null;
        }

        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(npc.aggroList);

        foreach (var character in tempCharacters) {
            NetworkBehaviour _character = character as NetworkBehaviour;
            ExitCombat(_character);
        }

        ResetValues();
    }
    #endregion
    #region Combat Logic
    [Server(Logging = LoggingType.Off)]
    private void HandleCombatState() {
        CheckForEnemiesInRange();
        if (npc.combatStatus == CombatStatus.OutOfCombat) {
            npc.location = transform.position;
        }
        if (npc.targetStatus == TargetStatus.Dead && npc.currentHealth > 0) {
            meshShatter.ResetCharacter();
            StartCoroutine(ResetPosition());
        } else if (npc.combatStatus == CombatStatus.InCombat && npc.targetStatus != TargetStatus.Dead) {
            StartCoroutine(InCombat());
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void CheckForEnemiesInRange() {
        // Logic to detect players within aggro range and add them to combat list
        if (npc.currentHealth > 0 && npc.combatStatus != CombatStatus.Resetting && npc.combatStatus != CombatStatus.InCombat) {
            foreach (var target in FindObjectsOfType<MonoBehaviour>().OfType<ICombatable>().Where(c => !ReferenceEquals(c, this))) {
                GameObject targetObject = target.GetTargetObject();
                if (Vector3.Distance(transform.position, targetObject.transform.position) <= CalculateCombatRange(target) && IsLineOfSightClear(targetObject) && target.GetTargetStatus() != TargetStatus.Dead) {
                    NetworkBehaviour _target = target as NetworkBehaviour;
                    EnterCombat(_target);
                }
            }
        }
    }

    [Server(Logging = LoggingType.Off)]
    private IEnumerator InCombat() {
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<ICombatable>(npc.aggroList);
        for (int i = 0; i < tempCharacters.Count; i++) {
            if (ShouldStopCombat(tempCharacters[i])) {
                if ((tempCharacters[i] as NetworkBehaviour).IsDestroyed()) {
                    npc.aggroList.RemoveAt(i);
                } else {
                    ExitCombat(tempCharacters[i] as NetworkBehaviour);
                }
            }
        }
        if (npc.aggroList.Count == 0) {
            StartCoroutine(ResetPosition());
        } else {
            npc.combatStatus = CombatStatus.InCombat;
            var target = npc.aggroList[0];
            if (npc.actionState == ActionState.Idle) {
                npc.actionState = ActionState.AutoAttacking;
            }
            if (npc.actionState == ActionState.AutoAttacking) {
                HandleAutoAttack(target);
            }
            // Look toward the target logic goes here
            if (!(target as NetworkBehaviour).IsDestroyed()) {
                GameObject targetObject = target.GetTargetObject();
                Vector3 directionToTarget = targetObject.transform.position - transform.position;
                directionToTarget.y = 0; // This ensures the rotation only happens along the y-axis
                Quaternion targetRotation = Quaternion.identity;
                if (directionToTarget != Vector3.zero) {
                   targetRotation  = Quaternion.LookRotation(directionToTarget);
                }
                // Apply the rotation to the gameObject
                transform.rotation = targetRotation;
                if (IsTargetInAttackRange(targetObject) && IsLineOfSightClear(targetObject)) {
                    //agent.SetDestination(transform.position);
                } else {
                    PathToTarget(targetObject);
                }
            }
            
            yield return null;
        }
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateCombatRange(ICombatable _target) {
        Character target = Database.instance.GetTarget(_target);
        if (target == null) {
            return 0;
        }
        int levelDifference = Mathf.Abs(target.level - npc.level);
        if (target.level >= npc.level) {
            return npc.aggroRange - Mathf.Min(levelDifference, 5);
        } else {
            return npc.aggroRange + Mathf.Min(levelDifference, 5);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInAttackRange(GameObject target) {
        if (target == null) return true;
        return Vector3.Distance(transform.position, target.transform.position) <= npc.autoAttackRange - 2;
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsLineOfSightClear(GameObject target) {
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f) return true;
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.position;
        float range = (npc.combatStatus == CombatStatus.InCombat) ? npc.autoAttackRange : npc.aggroRange;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, range)) {
            return hit.collider.gameObject == target;
        }
        return false;
    }

    #endregion

    #region Auto Attack Logic

    [Server(Logging = LoggingType.Off)]
    private void HandleAutoAttack(ICombatable target) {
        if (IsTargetInRangeAndVisible(target)) {
            if (npc.autoAttackTimer <= 0f) {
                PerformAutoAttack(target);
                npc.autoAttackTimer = CalculateAutoAttackCooldown();
            } else {
                npc.autoAttackTimer = Mathf.Max(npc.autoAttackTimer - Time.deltaTime, 0);
            }
        } else {
            npc.autoAttackTimer = Mathf.Max(npc.autoAttackTimer - Time.deltaTime, 0);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void PerformAutoAttack(ICombatable _target) {
        Character target = Database.instance.GetTarget(_target);
        if (target != null && target.currentHealth > 0) {
            // Auto-attack logic here...
            Debug.Log("Performed");
            int damage = UnityEngine.Random.Range(npc.minAutoAttackDamage, npc.maxAutoAttackDamage);
            combatManager.SendDamage(_target, damage);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInRangeAndVisible(ICombatable target) {
        if ((target as NetworkBehaviour).IsDestroyed()) return false;

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

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown() {
        return Mathf.Max(npc.autoAttackCooldown, 0.5f);
    }

    #endregion

    #region Combat Logic
    // New method to determine whether to stop combat
    [Server(Logging = LoggingType.Off)]
    private bool ShouldStopCombat(ICombatable target) {
        if ((target as NetworkBehaviour).IsDestroyed()) return true;
        return Vector3.Distance(npc.location, target.GetTargetObject().transform.position) > npc.maxAttackRange;
    }

    // Overriding methods...

    [ObserversRpc]
    private void EnterCombat(NetworkBehaviour target) {
        ICombatable combatant = target as ICombatable;
        if (!npc.aggroList.Contains(combatant)) {
            npc.aggroList.Add(combatant);
            npc.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            if (!target.IsDestroyed() && target.IsOwner) combatant.ServerEnterCombat(this);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerEnterCombat(NetworkBehaviour target) {
        EnterCombat(target);
    }

    [ObserversRpc]
    public void ExitCombat(NetworkBehaviour target) {
        ICombatable combatant = target as ICombatable;
        if (npc.aggroList.Contains(combatant)) {
            npc.aggroList.Remove(combatant);

            // Notify the other character to exit combat
            if (!target.IsDestroyed() && target.IsOwner) combatant.ServerExitCombat(this);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerExitCombat(NetworkBehaviour target) {
        ExitCombat(target);
    }

    [Server(Logging = LoggingType.Off)]
    public void ExitAllCombat() {
        var tempTargets = new List<ICombatable>(npc.aggroList);
        foreach (var target in tempTargets) {
            Character character = Database.instance.GetTarget(target);
            if (character as Player != null) {
                Player player = character as Player;
                player.currentExperience += npc.experience;
            }
            ExitCombat(target as NetworkBehaviour);
        }
    }


    #endregion

    #region Death Logic
    [Server(Logging = LoggingType.Off)]
    private void Death() {
        Debug.Log("Running Death");
        npc.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempTargets = new List<ICombatable>(npc.aggroList);
        //PlayerBehaviour tempPlayer = null;

        foreach (var target in tempTargets) {
            Character character = Database.instance.GetTarget(target);
            if (character as Player != null) {
                Player player = character as Player;
                player.currentExperience += npc.experience;
            }
            ExitCombat(target as NetworkBehaviour);
        }
        // Stop all coroutines to cease current behaviour
        StopAllCoroutines();

        // Disable the NavMeshAgent to stop movement
        if (agent != null) {
            agent.enabled = false;
        }

        if (meshShatter) {
            meshShatter.ShatterCharacter();
        }

        // Additional death behaviour (e.g., playing death animation, dropping items, etc.)
        // ...

        // Optionally disable the GameObject or components to simulate death
        // gameObject.SetActive(false);
        // Or disable relevant components, e.g., colliders, renderers, etc.
    }
    #endregion

    #region Interfaces
    public Key GetKey() {
        return npc.key;
    }

    public Character GetTarget() {
        return npc;
    }

    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return npc.targetType;
    }

    public TargetStatus GetTargetStatus() {
        return npc.targetStatus;
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }

    [Server(Logging = LoggingType.Off)]
    private void OnApplicationQuit() {
        npc.currentHealth = npc.maxHealth;
        npc.currentMana = npc.maxMana;
    }
    #endregion
}