using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity {
    Normal,
    Rare,
    Elite,
    Boss
}

[Serializable]
public class NPC : Character {
    public NPCBehaviour npcBehaviour;
    public float maxAttackRange = 50f;
    public float aggroRange = 10f; // Base range for initiating combat
    public Vector3 lastAttackPosition; // Last position where the NPC attacked the player
    public Rarity rarity;
    public int wealth;
    public float experience;
    public List<Item> dropTable;

    public override void Sync() {
        if (npcBehaviour != null) {
            npcBehaviour.npc.Dirty();
        }
    }

    public void SetNPC(NPC _npc) {
        // Copying basic types and structures (value types)
        maxAttackRange = _npc.maxAttackRange;
        aggroRange = _npc.aggroRange;
        lastAttackPosition = _npc.lastAttackPosition;
        rarity = _npc.rarity;
        wealth = _npc.wealth;
        experience = _npc.experience;

        // For the DropTable, which is a List of Items, we want to make a new list and copy each item.
        // Assuming 'Item' is a class you need to properly clone each item if it's not a simple type
        // For a deep copy, you would need a method in your Item class to clone or copy its properties.
        // Here's a shallow copy example, which works if Items are immutable or you're okay with shared references
        if (dropTable != null ) {
            dropTable = new List<Item>(_npc.dropTable);
        } else {
            dropTable = new List<Item>();
        }
        // Copying inherited properties from Character
        name = _npc.name;
        ID = _npc.ID;
        targetType = _npc.targetType;
        characterRace = _npc.characterRace;
        targetStatus = _npc.targetStatus;
        combatStatus = _npc.combatStatus;
        actionState = _npc.actionState;
        level = _npc.level;
        maxHealth = _npc.maxHealth;
        currentHealth = _npc.currentHealth;
        maxMana = _npc.maxMana;
        currentMana = _npc.currentMana;
        armor = _npc.armor;
        minAutoAttackDamage = _npc.minAutoAttackDamage;
        maxAutoAttackDamage = _npc.maxAutoAttackDamage;
        autoAttackRange = _npc.autoAttackRange;
        speed = _npc.speed;
        autoAttackCooldown = _npc.autoAttackCooldown;
        autoAttackTimer = _npc.autoAttackTimer;
        gameObject = _npc.gameObject; // Be careful with GameObjects; you usually don't want to copy these directly

        // Deep copying lists, ensuring a new list is created and filled with copies of the original items
        if (_npc.weaknesses != null) {
            weaknesses = new List<DamageTypes>(_npc.weaknesses);
        } else {
            weaknesses = new List<DamageTypes>();
        }

        if (_npc.resistances != null) {
            resistances = new List<DamageTypes>(_npc.resistances);
        } else {
            resistances = new List<DamageTypes>();
        }

        if (_npc.immunities != null) {
            immunities = new List<DamageTypes>(_npc.immunities);
        } else {
            immunities = new List<DamageTypes>();
        }

        if (_npc.aggroList != null) {
            aggroList = new List<GameObject>(_npc.aggroList); // This is also a shallow copy; consider implications in your game logic
        } else {
            _npc.aggroList = new List<GameObject>();
        }
    }
}