using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCBehaviour : EntityBehaviour, ICombatable, ITargetable, ICastable, IAbleToAttack, IAbleToCast {
    #region Variables
    public NPC npcData;
    public float moveSpeed = 7.1f;
    public float maxAttackRange = 50f;
    private float autoAttackTimer = 0f;
    private NavMeshAgent agent;
    public float aggroRange = 10f; // Base range for initiating combat
    private Vector3 lastAttackPosition; // Last position where the NPC attacked the player
    private Vector3 originalPosition;
    private TargetType initialStatus;
    public int ID;
    #endregion

    public override void OnStartServer() {
        base.OnStartServer();
        if (characterData != null) {
            npcData = characterData as NPC;
        }
        if (npcData == null) {
            return;
        }
        agent = GetComponent<NavMeshAgent>();
        SetStartingValues();
        originalPosition = transform.position;

        // Initialize based on npcData
        agent.speed = moveSpeed;

        // Adjust NavMeshAgent settings for NPC collision
        agent.radius = 0.5f; // Adjust as needed for the size of your NPCs
        agent.height = 1.8f; // Adjust based on NPC height

        // Set the obstacle avoidance type to none or low
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    protected override void Update() {
        base.Update();
        CheckForEnemiesInRange();
        HandleCombatState();
    }

    public void SetStartingValues() {
        npcData.currentHealth = npcData.maxHealth;
        npcData.currentMana = npcData.maxMana;
    }

    #region Movement Logic
    private void PathToTarget(GameObject target) {
        agent.SetDestination(target.transform.position);
    }

    private IEnumerator ResetPosition() {
        state = State.Resetting;

        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<EntityBehaviour>(aggroList);

        foreach (var character in tempCharacters) {
            ExitCombatWith(character.GetComponent<EntityBehaviour>());
        }
        agent.enabled = false;

        while (Vector3.Distance(transform.position, originalPosition) > 0f) {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetStartingValues();
        agent.enabled = true;
        state = State.OutOfCombat;
    }
    #endregion
    #region Combat Logic
    private void HandleCombatState() {
        if (state == State.OutOfCombat) {
            originalPosition = transform.position;
        }
        if (state == State.Dead && npcData.currentHealth > 0) {
            StartCoroutine(ResetPosition());
         } else if (npcData.targetType == TargetType.Hostile && state != State.Resetting && state != State.Dead) {
            StartCoroutine(Combat());
        }
    }

    private void CheckForEnemiesInRange() {
        // Logic to detect players within aggro range and add them to combat list
        if (npcData.currentHealth > 0 && state != State.Resetting) {
            foreach (var player in FindObjectsOfType<PlayerBehaviour>()) {
                /*
                if (player.playerData.Value) {
                    if (Vector3.Distance(transform.position, player.transform.position) <= CalculateCombatRange(player) && IsLineOfSightClear(player.gameObject) && player.state != State.Dead) {
                        EnterCombatWith(player);
                    }
                }
                */
            }
        }
    }

    private IEnumerator Combat() {
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<EntityBehaviour>(aggroList);
        foreach (var character in tempCharacters) {
            if (ShouldStopCombat(character)) {
                ExitCombatWith(character.GetComponent<EntityBehaviour>());
            }
        }
        if (aggroList.Count == 0) {
            StartCoroutine(ResetPosition());
        } else {
            state = State.Combat;
            //Debug.Log("Attacking");
            var targetBehavior = aggroList[0];
            var target = targetBehavior.gameObject;
            HandleAutoAttack(targetBehavior);
            // Look toward the target logic goes here
            if (target != null) {
                Vector3 directionToTarget = target.transform.position - transform.position;
                directionToTarget.y = 0; // This ensures the rotation only happens along the y-axis
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
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

    private float CalculateCombatRange(EntityBehaviour target) {
        int levelDifference = Mathf.Abs(target.GetComponent<EntityBehaviour>().characterData.level - npcData.level);
        if (target.GetComponent<EntityBehaviour>().characterData.level >= npcData.level) {
            return aggroRange - Mathf.Min(levelDifference, 5);
        } else {
            return aggroRange + Mathf.Min(levelDifference, 5);
        }
    }
    private bool IsTargetInAttackRange(GameObject target) {
        return Vector3.Distance(transform.position, target.transform.position) <= npcData.autoAttackRange - 2;
    }

    private bool IsLineOfSightClear(GameObject target) {
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.position;
        float range = (state == State.Combat) ? npcData.autoAttackRange : aggroRange;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, range)) {
            return hit.collider.gameObject == target;
        }
        return false;
    }

    #region Auto Attack Logic

    private void HandleAutoAttack(EntityBehaviour target) {
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

    private void PerformAutoAttack(EntityBehaviour target) {
        if (target != null && target.characterData.currentHealth > 0) {
            // Auto-attack logic here...
            Debug.Log("Performed");
            int damage = Random.Range(npcData.autoAttackDamageMin, npcData.autoAttackDamageMax);
            combatManager.SendDamage(target, damage);
        }
    }

    private bool IsTargetInRangeAndVisible(EntityBehaviour target) {
        if (target == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget > npcData.autoAttackRange) return false;

        // Calculate direction to target
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Check if target is in front of the player
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget > 45) return false; // Assuming 90 degree field of view (45 degrees on either side of forward direction)

        // Perform raycast to check line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, npcData.autoAttackRange)) {
            return hit.collider.gameObject == target.gameObject;
        }

        return false;
    }

    private float CalculateAutoAttackCooldown() {
        return Mathf.Max(npcData.autoAttackCooldown, 0.5f);
    }

    #endregion


    // New method to determine whether to stop combat
    private bool ShouldStopCombat(EntityBehaviour target) {
        return Vector3.Distance(originalPosition, target.transform.position) > maxAttackRange;
    }

    // Overriding methods...
    public override void EnterCombatWith(EntityBehaviour entityBehaviour) {
        base.EnterCombatWith(entityBehaviour);
        // NPC specific logic for entering combat
    }

    public override void ExitCombatWith(EntityBehaviour entityBehaviour) {
        base.ExitCombatWith(entityBehaviour);
        // NPC specific logic for exiting combat
    }

    public void EnterCombat() {

    }

    public void ExitCombat() {

    }

#endregion
    #region Death Logic
    protected override void Death() {
        Debug.Log("Running Death");
        state = State.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<EntityBehaviour>(aggroList);
        //PlayerBehaviour tempPlayer = null;

        foreach (var character in tempCharacters) {
            /*tempPlayer = character as PlayerBehaviour;
            if (tempPlayer != null) {
                tempPlayer.playerData.Value.currentExperience += npcData.experience;
            }*/
            ExitCombatWith(character.GetComponent<EntityBehaviour>());
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

    public Identifiable GetTarget() {
        return Database.instance.GetNPC(ID);
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
        npcData.currentHealth = npcData.maxHealth;
        npcData.currentMana = npcData.maxMana;
    }
}