using UnityEngine;
using UnityEngine.AI;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using Unity.VisualScripting;
using FishNet.Object.Synchronizing;
using FishNet.CodeGenerating;
using FishNet.Demo.AdditiveScenes;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MeshShatter))]
public class NPCBehaviour : NetworkBehaviour, ICombatable, ICastable, IAbleToAttack, IAbleToCast {
    #region Variables
    protected MeshShatter meshShatter;
    public CombatManager combatManager;
    public NavMeshAgent agent;
    public ScriptableNPC scriptableNPC;
    [AllowMutableSyncType] public SyncVar<NPC> npc = new SyncVar<NPC>();
    public readonly SyncVar<bool> isReady = new SyncVar<bool>(false);
    public Vector3 startingPosition;
    public Quaternion startingRotation;
    public Vector3 startingScale;
    #endregion

    private void Awake() {
        npc.OnChange += npc_OnChange;
    }

    private void npc_OnChange(NPC oldPlayer, NPC newPlayer, bool asServer) {
        if (asServer) {
            if (!isReady.Value) {
                isReady.Value = true;
            }
        } else {
            SetReady();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReady() {
        if (!isReady.Value) {
            isReady.Value = true;
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();

        SetStartingValues();

        meshShatter = GetComponent<MeshShatter>();
        combatManager = FindObjectOfType<CombatManager>();
        agent = GetComponent<NavMeshAgent>();

        // Initialize based on npc
        agent.speed = npc.Value.speed;

        agent.radius = 0.5f; // Adjust as needed for the size of your NPCs
        agent.height = 1.8f; // Adjust based on NPC height

        // Set the obstacle avoidance type to none or low
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

    }

    public override void OnStartClient() {
        Sync();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Sync() {
        npc.Value.Sync();
    }

    [Server(Logging = LoggingType.Off)]
    private void Update() {
        if (!base.IsServerInitialized || !isReady.Value) return;

        if (npc.Value.currentHealth <= 0 && npc.Value.targetStatus != TargetStatus.Dead) {
            Death();
        } else {
            HandleCombatState();
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void SetStartingValues() {
        npc.Value.SetNPC(scriptableNPC.npc);
        npc.Value.gameObject = gameObject;
        npc.Value.currentTarget = null;
        npc.Value.npcBehaviour = this;
        startingPosition = gameObject.transform.position;
        startingRotation = gameObject.transform.rotation;
        startingScale = gameObject.transform.localScale;
        npc.Value.Sync();
    }

    [Server(Logging = LoggingType.Off)]
    public void ResetValues() {
        agent.enabled = true;
        npc.Value.currentHealth = npc.Value.maxHealth;
        npc.Value.currentMana = npc.Value.maxMana;
        transform.position = startingPosition;
        transform.localScale = startingScale;
        transform.rotation = startingRotation;
        npc.Value.targetStatus = TargetStatus.Alive;
        npc.Value.combatStatus = CombatStatus.OutOfCombat;
        npc.Value.actionState = ActionState.Idle;
        npc.Value.Sync();
    }

    #region Movement Logic
    [Server(Logging = LoggingType.Off)]
    private void PathToTarget(GameObject target) {
        if (agent != null && target != null) {
            agent.SetDestination(target.transform.position);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private IEnumerator ResetPosition() {
        agent.enabled = false;
        npc.Value.combatStatus = CombatStatus.Resetting;

        while (Vector3.Distance(transform.position, startingPosition) > 0f) {
            Vector3 direction = startingPosition - transform.position;
            direction.y = 0; // This ensures the rotation only happens along the y-axis
            Quaternion targetRotation = Quaternion.identity;
            if (direction != Vector3.zero) {
                targetRotation = Quaternion.LookRotation(direction);
            }
            // Apply the rotation to the gameObject
            transform.rotation = targetRotation;
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, npc.Value.speed * Time.deltaTime);
            yield return null;
        }

        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<GameObject>(npc.Value.aggroList);

        foreach (var character in tempCharacters) {
            ExitCombat(character);
        }
        ResetValues();
    }
    #endregion
    #region Combat Logic
    [Server(Logging = LoggingType.Off)]
    private void HandleCombatState() {
        if (npc.Value.combatStatus == CombatStatus.OutOfCombat) {
            CheckForEnemiesInRange();
            startingPosition = gameObject.transform.position;
        }
        if (npc.Value.targetStatus == TargetStatus.Dead && npc.Value.currentHealth > 0) {
            meshShatter.ResetCharacter();
            StartCoroutine(ResetPosition());
        } else if (npc.Value.combatStatus == CombatStatus.InCombat && npc.Value.targetStatus != TargetStatus.Dead) {
            StartCoroutine(InCombat());
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void CheckForEnemiesInRange() {
        if (npc.Value.currentHealth > 0 && npc.Value.combatStatus != CombatStatus.Resetting && npc.Value.combatStatus != CombatStatus.InCombat) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, npc.Value.aggroRange);
            foreach (var hitCollider in hitColliders) {
                GameObject target = hitCollider.gameObject;
                if (target.GetComponent<ICombatable>() != null && !ReferenceEquals(target, this)) {
                    if (Vector3.Distance(transform.position, target.transform.position) <= CalculateAggroRange(target) && IsLineOfSightClear(target) && CanAggroTarget(target.GetComponent<ICombatable>())) {
                        EnterCombat(target);
                    }
                }
            }
        }
    }

    [Server(Logging = LoggingType.Off)]
    private bool CanAggroTarget(ICombatable target) {
        if (target == null)
            return false;
        if (npc.Value.targetStatus != TargetStatus.Dead && target.GetTargetStatus() != TargetStatus.Dead) {
            if (npc.Value.targetType == TargetType.Hostile && (target.GetTargetType() == TargetType.Player || 
                target.GetTargetType() == TargetType.Companion || target.GetTargetType() == TargetType.Ally)) {
                return true;
            }
        }
        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private IEnumerator InCombat() {
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<GameObject>(npc.Value.aggroList);
        for (int i = 0; i < tempCharacters.Count; i++) {
            if (ShouldStopCombat(tempCharacters[i])) {
                if (tempCharacters[i].IsDestroyed()) {
                    npc.Value.aggroList.RemoveAt(i);
                } else {
                    ExitCombat(tempCharacters[i]);
                }
            }
        }
        if (npc.Value.aggroList.Count == 0) {
            StartCoroutine(ResetPosition());
        } else {
            var target = npc.Value.aggroList[0];
            if (npc.Value.actionState == ActionState.Idle) {
                npc.Value.actionState = ActionState.AutoAttacking;
            }
            if (npc.Value.actionState == ActionState.AutoAttacking) {
                HandleAutoAttack(target);
            }
            // Look toward the target logic goes here
            if (!target.IsDestroyed()) {
                Vector3 directionToTarget = target.transform.position - transform.position;
                directionToTarget.y = 0; // This ensures the rotation only happens along the y-axis
                Quaternion targetRotation = Quaternion.identity;
                if (directionToTarget != Vector3.zero) {
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                }
                // Apply the rotation to the gameObject
                transform.rotation = targetRotation;
                if (IsTargetInAttackRange(target) && IsLineOfSightClear(target)) {
                    agent.SetDestination(transform.position);
                } else {
                    PathToTarget(target);
                }
            }

            yield return null;
        }
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAggroRange(GameObject _target) {
        Character target = _target.GetComponent<ICombatable>().GetTarget();
        if (target == null) {
            return 0;
        }
        int levelDifference = Mathf.Abs(target.level - npc.Value.level);
        if (target.level >= npc.Value.level) {
            return npc.Value.aggroRange - Mathf.Min(levelDifference, 5);
        } else {
            return npc.Value.aggroRange + Mathf.Min(levelDifference, 5);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInAttackRange(GameObject target) {
        if (target == null) return true;
        return Vector3.Distance(transform.position, target.transform.position) <= npc.Value.autoAttackRange - 2;
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsLineOfSightClear(GameObject target) {
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f) return true;
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.position;
        float range = (npc.Value.combatStatus == CombatStatus.InCombat) ? npc.Value.autoAttackRange : npc.Value.aggroRange;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, range)) {
            return hit.collider.gameObject == target;
        }
        return false;
    }

    #endregion

    #region Auto Attack Logic

    [Server(Logging = LoggingType.Off)]
    private void HandleAutoAttack(GameObject target) {
        if (IsTargetInRangeAndVisible(target) && npc.Value.autoAttackTimer <= 0f) {
            PerformAutoAttack(target);
            npc.Value.autoAttackTimer = CalculateAutoAttackCooldown();
            npc.Value.Sync();
        } else if (npc.Value.autoAttackTimer > 0) {
            npc.Value.autoAttackTimer -= Time.deltaTime;
            npc.Value.Sync();
        }
    }

    [Server(Logging = LoggingType.Off)]
    private void PerformAutoAttack(GameObject _target) {
        Character target = _target.GetComponent<ICombatable>().GetTarget();
        if (target != null && target.currentHealth > 0) {
            // Auto-attack logic here...
            int damage = Random.Range(npc.Value.minAutoAttackDamage, npc.Value.maxAutoAttackDamage);
            combatManager.SendDamage(_target.GetComponent<ICombatable>(), damage);
        }
    }

    [Server(Logging = LoggingType.Off)]
    private bool IsTargetInRangeAndVisible(GameObject target) {
        if (target.IsDestroyed()) return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget > npc.Value.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, npc.Value.autoAttackRange)) {
            return hit.collider.gameObject == target;
        }

        return false;
    }

    [Server(Logging = LoggingType.Off)]
    private float CalculateAutoAttackCooldown() {
        return Mathf.Max(npc.Value.autoAttackCooldown, 0.5f);
    }

    #endregion

    #region Combat Logic
    // New method to determine whether to stop combat
    [Server(Logging = LoggingType.Off)]
    private bool ShouldStopCombat(GameObject target) {
        if (target.IsDestroyed()) return true;
        return Vector3.Distance(startingPosition, target.transform.position) > npc.Value.maxAttackRange;
    }

    // Overriding methods...

    [Server(Logging = LoggingType.Off)]
    private void EnterCombat(GameObject target) {
        if (!npc.Value.aggroList.Contains(target)) {
            npc.Value.aggroList.Add(target);
            npc.Value.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            if (!target.IsDestroyed()) target.GetComponent<ICombatable>().ServerEnterCombat(gameObject);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerEnterCombat(GameObject target) {
        EnterCombat(target);
    }

    [Server(Logging = LoggingType.Off)]
    public void ExitCombat(GameObject target) {
        if (npc.Value.aggroList.Contains(target)) {
            npc.Value.aggroList.Remove(target);
            // Notify the other character to exit combat
            if (!target.IsDestroyed()) target.GetComponent<ICombatable>().ServerExitCombat(gameObject);
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void ServerExitCombat(GameObject target) {
        ExitCombat(target);
    }

    [Server(Logging = LoggingType.Off)]
    public void ExitAllCombat() {
        var tempTargets = new List<GameObject>(npc.Value.aggroList);
        foreach (var target in tempTargets) {
            Character character = target.GetComponent<ICombatable>().GetTarget();
            if (character as Player != null) {
                Player player = character as Player;
                player.currentExperience += npc.Value.experience;
            }
            ExitCombat(target);
        }
    }


    #endregion

    #region Death Logic
    [Server(Logging = LoggingType.Off)]
    private void Death() {
        npc.Value.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        ExitAllCombat();
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
        return npc.Value.GetKey();
    }

    public Character GetTarget() {
        return npc.Value;
    }

    public ITargetable GetTargetComponent() {
        return this;
    }

    public TargetType GetTargetType() {
        return npc.Value.targetType;
    }

    public TargetStatus GetTargetStatus() {
        return npc.Value.targetStatus;
    }

    public GameObject GetTargetObject() {
        return gameObject;
    }

    [Server(Logging = LoggingType.Off)]
    private void OnApplicationQuit() {
        npc.Value.currentHealth = npc.Value.maxHealth;
        npc.Value.currentMana = npc.Value.maxMana;
    }
    #endregion
}