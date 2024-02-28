/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System;

[RequireComponent(typeof(DeathShatter))]
public class EntityBehaviour : NetworkBehaviour {
    protected DeathShatter deathShatter;
    public List<EntityBehaviour> aggroList = new List<EntityBehaviour>();
    public CombatManager combatManager;

    protected virtual void Awake() {
        deathShatter = GetComponent<DeathShatter>();
        combatManager = FindObjectOfType<CombatManager>();
    }

    protected virtual void Update() {
        if (!base.IsServerInitialized)
            return;

        if (characterData == null) {
            return;
        }
        if (characterData.currentHealth <= 0 && (characterData.targetStatus != TargetStatus.Dead && characterData.combatStatus != CombatStatus.Resetting)) {
            Death();
        } else if (characterData.targetStatus != TargetStatus.Dead && characterData.combatStatus != CombatStatus.Resetting) {
            if (characterData != null) {
                if (characterData.currentHealth > characterData.maxHealth) {
                    characterData.currentHealth = characterData.maxHealth;
                } else if (characterData.currentHealth < 0) {
                    characterData.currentHealth = 0;
                }
            }
        } else {
            return;
        }
    }

    public virtual void EnterCombatWith(EntityBehaviour entityBehaviour) {
        if (!aggroList.Contains(entityBehaviour)) {
            aggroList.Add(entityBehaviour);
            characterData.combatStatus = CombatStatus.InCombat;

            // Notify the other character to enter combat
            entityBehaviour.EnterCombatWith(this);
        }
    }

    public virtual void ExitCombatWith(EntityBehaviour entityBehaviour) {
        if (aggroList.Contains(entityBehaviour)) {
            aggroList.Remove(entityBehaviour);

            // Notify the other character to exit combat
            entityBehaviour.ExitCombatWith(this);
        }
    }

    protected virtual void Death() {
        characterData.targetStatus = TargetStatus.Dead;
        // Create a temporary list to store characters to exit combat with
        var tempCharacters = new List<EntityBehaviour>(aggroList);

        foreach (var character in tempCharacters) {
            ExitCombatWith(character.GetComponent<EntityBehaviour>());
        }

        if (deathShatter) {
            deathShatter.ShatterCharacter();
        }
    }
}
*/