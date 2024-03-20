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

    [Server(Logging = LoggingType.Off)]
    public void SetNPC(NPC scriptableNPC) {
        // Copying basic types and structures (value types)
        maxAttackRange = scriptableNPC.maxAttackRange;
        aggroRange = scriptableNPC.aggroRange;
        lastAttackPosition = scriptableNPC.lastAttackPosition;
        rarity = scriptableNPC.rarity;
        wealth = scriptableNPC.wealth;
        experience = scriptableNPC.experience;

        // For the DropTable, which is a List of Items, we want to make a new list and copy each item.
        // Assuming 'Item' is a class you need to properly clone each item if it's not a simple type
        // For a deep copy, you would need a method in your Item class to clone or copy its properties.
        // Here's a shallow copy example, which works if Items are immutable or you're okay with shared references
        if (dropTable != null ) {
            dropTable = new List<Item>(scriptableNPC.dropTable);
        } else {
            dropTable = new List<Item>();
        }
        // Copying inherited properties from Character
        name = scriptableNPC.name;
        ID = scriptableNPC.ID;
        targetType = scriptableNPC.targetType;
        characterRace = scriptableNPC.characterRace;
        targetStatus = scriptableNPC.targetStatus;
        combatStatus = scriptableNPC.combatStatus;
        actionState = scriptableNPC.actionState;
        level = scriptableNPC.level;
        maxHealth = scriptableNPC.maxHealth;
        currentHealth = scriptableNPC.currentHealth;
        maxMana = scriptableNPC.maxMana;
        currentMana = scriptableNPC.currentMana;
        armor = scriptableNPC.armor;
        minAutoAttackDamage = scriptableNPC.minAutoAttackDamage;
        maxAutoAttackDamage = scriptableNPC.maxAutoAttackDamage;
        autoAttackRange = scriptableNPC.autoAttackRange;
        speed = scriptableNPC.speed;
        autoAttackCooldown = scriptableNPC.autoAttackCooldown;
        autoAttackTimer = scriptableNPC.autoAttackTimer;
        gameObject = scriptableNPC.gameObject; // Be careful with GameObjects; you usually don't want to copy these directly

        // Deep copying lists, ensuring a new list is created and filled with copies of the original items
        if (scriptableNPC.weaknesses != null) {
            weaknesses = new List<DamageTypes>(scriptableNPC.weaknesses);
        } else {
            weaknesses = new List<DamageTypes>();
        }

        if (scriptableNPC.resistances != null) {
            resistances = new List<DamageTypes>(scriptableNPC.resistances);
        } else {
            resistances = new List<DamageTypes>();
        }

        if (scriptableNPC.immunities != null) {
            immunities = new List<DamageTypes>(scriptableNPC.immunities);
        } else {
            immunities = new List<DamageTypes>();
        }

        if (scriptableNPC.aggroList != null) {
            aggroList = new List<GameObject>(scriptableNPC.aggroList); // This is also a shallow copy; consider implications in your game logic
        } else {
            scriptableNPC.aggroList = new List<GameObject>();
        }
    }
}