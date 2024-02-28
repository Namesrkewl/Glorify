using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NPC", menuName = "Character/NPC")]
public class NPC : ScriptableObject, Identifiable {
    [NonSerialized] public Image characterImage;
    public new string name;
    public TargetType targetType;
    public Races characterRace;
    public TargetStatus targetStatus;
    public CombatStatus combatStatus;
    public int level;
    public float maxHealth;
    public float currentHealth;
    public float maxMana;
    public float currentMana;
    public int armor;
    public int speed;
    public int autoAttackDamageMin;
    public int autoAttackDamageMax;
    public float autoAttackRange;
    public float autoAttackCooldown;
    public Vector3 location;
    public Vector3 scale;
    public Quaternion rotation;
    public List<DamageTypes> weaknesses;
    public List<DamageTypes> resistances;
    public List<DamageTypes> immunities;
    public Rarity rarity;
    public int wealth;
    public float experience;
    public List<Item> dropTable;
    [NonSerialized] public List<ICombatable> aggroList = new List<ICombatable>();
    private int npcID;

    public enum Rarity {
        Normal,
        Rare,
        Elite,
        Boss
    }

    // Other properties and methods specific to NPCs
    public int GetID() {
        return npcID;
    }

    public void SetID(int input) { 
        npcID = input;
    }

}