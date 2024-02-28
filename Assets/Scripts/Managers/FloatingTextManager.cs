using UnityEngine;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using System.Linq;

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
        List<ITargetable> entities = FindObjectsOfType<MonoBehaviour>().OfType<ITargetable>().ToList();
        GameObject entityObject;
        foreach (var entity in entities) {
            entityObject = entity.GetTargetObject();
            float distance = Vector3.Distance(player.transform.position, entityObject.transform.position);
            if (distance <= 50f && !activeEntities.Contains(entityObject.transform)) {
                CreateEntityPrefab(entity);
            } else if (distance > 50f && activeEntities.Contains(entityObject.transform)) {
                DestroyEntityPrefab(entity);
            }
        }
    }

    private void CreateEntityPrefab(ITargetable entity) {
        GameObject entityObject = entity.GetTargetObject();
        GameObject newEntity = Instantiate(entityPrefab, entityObject.transform.position, Quaternion.identity, transform);
        newEntity.name = entityObject.name;
        newEntity.GetComponent<EntityUI>().worldObjectTransform = entityObject.transform;
        activeEntities.Add(entityObject.transform);
        instantiatedPrefabs[entityObject.transform] = newEntity;
        // Additional setup for the new Entity prefab if needed
        CreateInformationText(newEntity.GetComponent<EntityUI>(), entity);
    }

    public EntityUI GetEntityUI(ITargetable entity) {
        GameObject entityObject = entity.GetTargetObject();
        if (instantiatedPrefabs.TryGetValue(entityObject.transform, out GameObject prefab)) {
            return prefab.GetComponent<EntityUI>();
        }
        return null;
    }

    private void DestroyEntityPrefab(ITargetable entity) {
        GameObject entityObject = entity.GetTargetObject();
        if (instantiatedPrefabs.TryGetValue(entityObject.transform, out GameObject prefab)) {
            Destroy(prefab);
            instantiatedPrefabs.Remove(entityObject.transform);
        }
        activeEntities.Remove(entityObject.transform);
    }

    public void CreateCombatText(EntityUI entityUI, string message) {
        if (entityUI != null) {
            Debug.Log("Test 2");
            entityUI.CreateCombatText(message);
        }
    }

    public void CreateInformationText(EntityUI entityUI, ITargetable entity) {
        if (entityUI != null) {
            entityUI.CreateInformationText(entity);
        }
    }
}