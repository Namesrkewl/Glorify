using FishNet.Demo.AdditiveScenes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character  {
    public PlayerClass playerClass;
    public Classes classEnum;
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
    public bool isSafe = true;
    public float regenerationCooldownTimer = 0f; // Timer for delaying regeneration
    public List<Spell> Spells;
    public List<Spell> Passives;

    public Player() {
        key = new Key();
        aggroList = new List<ICombatable>();
        level = 1;
        currentHealth = 100;
        maxHealth = 100;
        currentMana = 100;
        maxMana = 100;
        currentExperience = 0;
        maxExperience = 100;
        classEnum = Classes.Priest;
        playerClass = Resources.Load<PlayerClass>($"Player Classes/{classEnum}");
        location = new Vector3(2700, 12, 2200);
        scale = Vector3.one;
        rotation = Quaternion.identity;
        targetStatus = TargetStatus.Alive;
        combatStatus = CombatStatus.OutOfCombat;
        actionState = ActionState.Idle;
        vitality = 5;
        wisdom = 5;
    }
}