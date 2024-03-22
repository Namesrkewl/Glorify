using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : Character {
    public PlayerClass playerClass;
    public Classes classEnum;
    public PlayerBehaviour playerBehaviour;
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
    public bool isSafe;
    public float regenerationCooldownTimer = 0f; // Timer for delaying regeneration
    public List<Spell> spells;
    public List<Spell> passives;

    [Server(Logging = LoggingType.Off)]
    public void InitializePlayer() {
        aggroList = new List<GameObject>();
        level = 1;
        currentHealth = 100;
        maxHealth = 100;
        currentMana = 100;
        maxMana = 100;
        currentExperience = 0;
        maxExperience = 100;
        classEnum = Classes.Priest;
        playerClass = Resources.Load<PlayerClass>($"Player Classes/{classEnum}");
        targetStatus = TargetStatus.Alive;
        combatStatus = CombatStatus.OutOfCombat;
        actionState = ActionState.Idle;
        vitality = 5;
        wisdom = 5;
    }

    public override ITargetable GetBehaviour() {
        return playerBehaviour;
    }

    [Server(Logging = LoggingType.Off)]
    public override void Sync() {
        if (playerBehaviour != null) {
            playerBehaviour.player.Dirty();
        }
    }

    [Client(Logging = LoggingType.Off)]
    public override void SetInformationText(InformationText info) {
        playerBehaviour.informationText = info;
    }
}