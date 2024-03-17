using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : Character {
    [SerializeField] private PlayerClass playerClass;
    [SerializeField] private Classes classEnum;
    [SerializeField] private int strength;
    [SerializeField] private int dexterity;
    [SerializeField] private int intelligence;
    [SerializeField] private int criticalStrike;
    [SerializeField] private int haste;
    [SerializeField] private int wisdom;
    [SerializeField] private int vitality;
    [SerializeField] private int leech;
    [SerializeField] private int avoidance;
    [SerializeField] private int constitution;
    [SerializeField] private int dodge;
    [SerializeField] private int block;
    [SerializeField] private int parry;
    [SerializeField] private float maxExperience;
    [SerializeField] private float currentExperience;
    [SerializeField] private bool isSafe = true;
    [SerializeField] private float regenerationCooldownTimer = 0f; // Timer for delaying regeneration
    [SerializeField] private List<Spell> spells;
    [SerializeField] private List<Spell> passives;

    public Player() {
        SetAggroList(new List<ICombatable>());
        SetLevel(1);
        SetCurrentHealth(100);
        SetMaxHealth(100);
        SetCurrentMana(100);
        SetMaxMana(100);
        SetCurrentExperience(0);
        SetMaxExperience(100);
        SetClassEnum(Classes.Priest);
        SetPlayerClass(Resources.Load<PlayerClass>($"Player Classes/{classEnum}"));
        SetLocation(new Vector3(2700, 12, 2200));
        SetScale(Vector3.one);
        SetRotation(Quaternion.identity);
        SetTargetStatus(TargetStatus.Alive);
        SetCombatStatus(CombatStatus.OutOfCombat);
        SetActionState(ActionState.Idle);
        SetVitality(5);
        SetWisdom(5);
    }

    // Getters
    public PlayerClass GetPlayerClass() => playerClass;
    public Classes GetClassEnum() => classEnum;
    public int GetStrength() => strength;
    public int GetDexterity() => dexterity;
    public int GetIntelligence() => intelligence;
    public int GetCriticalStrike() => criticalStrike;
    public int GetHaste() => haste;
    public int GetWisdom() => wisdom;
    public int GetVitality() => vitality;
    public int GetLeech() => leech;
    public int GetAvoidance() => avoidance;
    public int GetConstitution() => constitution;
    public int GetDodge() => dodge;
    public int GetBlock() => block;
    public int GetParry() => parry;
    public float GetMaxExperience() => maxExperience;
    public float GetCurrentExperience() => currentExperience;
    public bool GetIsSafe() => isSafe;
    public float GetRegenerationCooldownTimer() => regenerationCooldownTimer;
    public List<Spell> GetSpells() => spells;
    public List<Spell> GetPassives() => passives;

    // Setters with Server attribute
    [Server(Logging = LoggingType.Off)]
    public void SetPlayerClass(PlayerClass _playerClass) => playerClass = _playerClass;

    [Server(Logging = LoggingType.Off)]
    public void SetClassEnum(Classes _classEnum) => classEnum = _classEnum;

    [Server(Logging = LoggingType.Off)]
    public void SetStrength(int _strength) => strength = _strength;

    [Server(Logging = LoggingType.Off)]
    public void SetDexterity(int _dexterity) => dexterity = _dexterity;

    [Server(Logging = LoggingType.Off)]
    public void SetIntelligence(int _intelligence) => intelligence = _intelligence;

    [Server(Logging = LoggingType.Off)]
    public void SetCriticalStrike(int _criticalStrike) => criticalStrike = _criticalStrike;

    [Server(Logging = LoggingType.Off)]
    public void SetHaste(int _haste) => haste = _haste;

    [Server(Logging = LoggingType.Off)]
    public void SetWisdom(int _wisdom) => wisdom = _wisdom;

    [Server(Logging = LoggingType.Off)]
    public void SetVitality(int _vitality) => vitality = _vitality;

    [Server(Logging = LoggingType.Off)]
    public void SetLeech(int _leech) => leech = _leech;

    [Server(Logging = LoggingType.Off)]
    public void SetAvoidance(int _avoidance) => avoidance = _avoidance;

    [Server(Logging = LoggingType.Off)]
    public void SetConstitution(int _constitution) => constitution = _constitution;

    [Server(Logging = LoggingType.Off)]
    public void SetDodge(int _dodge) => dodge = _dodge;

    [Server(Logging = LoggingType.Off)]
    public void SetBlock(int _block) => block = _block;

    [Server(Logging = LoggingType.Off)]
    public void SetParry(int _parry) => parry = _parry;

    [Server(Logging = LoggingType.Off)]
    public void SetMaxExperience(float _maxExperience) => maxExperience = _maxExperience;

    [Server(Logging = LoggingType.Off)]
    public void SetCurrentExperience(float _currentExperience) => currentExperience = _currentExperience;

    [Server(Logging = LoggingType.Off)]
    public void SetIsSafe(bool _isSafe) => isSafe = _isSafe;

    [Server(Logging = LoggingType.Off)]
    public void SetRegenerationCooldownTimer(float _regenerationCooldownTimer) => regenerationCooldownTimer = _regenerationCooldownTimer;

    [Server(Logging = LoggingType.Off)]
    public void SetSpells(List<Spell> _spells) => spells = _spells;

    [Server(Logging = LoggingType.Off)]
    public void SetPassives(List<Spell> _passives) => passives = _passives;
}