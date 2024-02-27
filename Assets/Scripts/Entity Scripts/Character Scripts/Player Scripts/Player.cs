using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Identifiable  {
    public PlayerClass playerClass;
    [System.NonSerialized] public Image characterImage;
    public string name;
    public TargetType targetType = TargetType.Player;
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
    public int strength;
    public int dexterity;
    public int intelligence;
    public int criticalStrike;
    public int haste;
    public int wisdom;
    public int vitality;
    public int leech;
    public int avoidance;
    public int constitution;
    public int dodge;
    public int block;
    public int parry;
    public float maxExperience;
    public float currentExperience;
    public float autoAttackTimer = 0f;
    public bool isSafe = false;
    public float regenerationCooldownTimer = 0f; // Timer for delaying regeneration
    public List<Spell> Spells;
    public List<Spell> Passives;
    [NonSerialized] public List<ICombatable> aggroList;
    private int playerID;

    public int GetID() {
        return playerID;
    }

    public void SetID(int input) {
        playerID = input;
    }
}