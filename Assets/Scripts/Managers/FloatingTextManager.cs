using UnityEngine;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;

public class FloatingTextManager : NetworkBehaviour {
    #region Variables
    public GameObject entityPrefab; // Prefab for the Entity
    public GameObject player;
    public int maxTexts = 50; // Max texts per entity

    private Dictionary<Transform, List<FloatingText>> activeTexts = new Dictionary<Transform, List<FloatingText>>();
    public HashSet<Transform> activeEntities = new HashSet<Transform>();
    private Dictionary<Transform, GameObject> instantiatedPrefabs = new Dictionary<Transform, GameObject>();
    #endregion

    void Update() {
        // No Player Connections Setup rn, do that later
        if (player) {
            CheckForEntitiesInRange();
        }
    }

    private void CheckForEntitiesInRange() {
        // Check for entities within range of the player
        EntityBehaviour[] entities = FindObjectsOfType<EntityBehaviour>();
        foreach (var entity in entities) {
            float distance = Vector3.Distance(player.transform.position, entity.transform.position);
            if (distance <= 50f && !activeEntities.Contains(entity.transform)) {
                CreateEntityPrefab(entity);
            } else if (distance > 50f && activeEntities.Contains(entity.transform)) {
                DestroyEntityPrefab(entity);
            }
        }
    }

    private void CreateEntityPrefab(EntityBehaviour entity) {
        GameObject newEntity = Instantiate(entityPrefab, entity.transform.position, Quaternion.identity, transform);
        newEntity.name = entity.name;
        newEntity.GetComponent<EntityUI>().worldObjectTransform = entity.transform;
        activeEntities.Add(entity.transform);
        instantiatedPrefabs[entity.transform] = newEntity;
        // Additional setup for the new Entity prefab if needed
        CreateInformationText(newEntity.GetComponent<EntityUI>(), entity);
    }

    public EntityUI GetEntityUI(EntityBehaviour entity) {
        if (instantiatedPrefabs.TryGetValue(entity.transform, out GameObject prefab)) {
            return prefab.GetComponent<EntityUI>();
        }
        return null;
    }

    private void DestroyEntityPrefab(EntityBehaviour entity) {
        if (instantiatedPrefabs.TryGetValue(entity.transform, out GameObject prefab)) {
            Destroy(prefab);
            instantiatedPrefabs.Remove(entity.transform);
        }
        activeEntities.Remove(entity.transform);
    }

    public void CreateCombatText(EntityUI entityUI, string message) {
        if (entityUI != null) {
            Debug.Log("Test 2");
            entityUI.CreateCombatText(message);
        }
    }

    public void CreateInformationText(EntityUI entityUI, EntityBehaviour entity) {
        if (entityUI != null) {
            entityUI.CreateInformationText(entity);
        }
    }
}