using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.UI;

public class Character : Identifiable {
    [SerializeField] private string name;
    [SerializeField] private int ID;
    public Key key = new Key();
    [NonSerialized] private Image image;
    [SerializeField] private TargetType targetType;
    [SerializeField] private Races characterRace;
    [SerializeField] private TargetStatus targetStatus;
    [SerializeField] private CombatStatus combatStatus;
    [SerializeField] private ActionState actionState;
    [SerializeField] private int level;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float currentMana;
    [SerializeField] private int armor;
    [SerializeField] private float speed;
    [SerializeField] private int minAutoAttackDamage;
    [SerializeField] private int maxAutoAttackDamage;
    [SerializeField] private float autoAttackRange;
    [SerializeField] private float autoAttackCooldown;
    [SerializeField] private float autoAttackTimer = 0f;
    [SerializeField] private Vector3 location;
    [SerializeField] private Vector3 scale;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private GameObject currentTarget;
    [SerializeField] private List<DamageTypes> weaknesses;
    [SerializeField] private List<DamageTypes> resistances;
    [SerializeField] private List<DamageTypes> immunities;
    [NonSerialized] private List<ICombatable> aggroList = new List<ICombatable>();

    // Getters
    public string GetName() => name;
    public int GetID() => ID;
    public Image GetImage() => image;
    public TargetType GetTargetType() => targetType;
    public Races GetCharacterRace() => characterRace;
    public TargetStatus GetTargetStatus() => targetStatus;
    public CombatStatus GetCombatStatus() => combatStatus;
    public ActionState GetActionState() => actionState;
    public int GetLevel() => level;
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxMana() => maxMana;
    public float GetCurrentMana() => currentMana;
    public int GetArmor() => armor;
    public float GetSpeed() => speed;
    public int GetMinAutoAttackDamage() => minAutoAttackDamage;
    public int GetMaxAutoAttackDamage() => maxAutoAttackDamage;
    public float GetAutoAttackRange() => autoAttackRange;
    public float GetAutoAttackCooldown() => autoAttackCooldown;
    public float GetAutoAttackTimer() => autoAttackTimer;
    public Vector3 GetLocation() => location;
    public Vector3 GetScale() => scale;
    public Quaternion GetRotation() => rotation;
    public GameObject GetCurrentTarget() => currentTarget;
    public List<DamageTypes> GetWeaknesses() => weaknesses;
    public List<DamageTypes> GetResistances() => resistances;
    public List<DamageTypes> GetImmunities() => immunities;
    public List<ICombatable> GetAggroList() => aggroList;

    // Setters with Server attribute
    [Server(Logging = LoggingType.Off)]
    public void SetName(string _name) {
        name = _name;
        key.name = name;
    }

    [Server(Logging = LoggingType.Off)]
    public void SetID(int _ID) {
        ID = _ID;
        key.ID = ID;
    }

    [Server(Logging = LoggingType.Off)]
    public void SetImage(Image _image) => image = _image;

    [Server(Logging = LoggingType.Off)]
    public void SetTargetType(TargetType _targetType) => targetType = _targetType;

    [Server(Logging = LoggingType.Off)]
    public void SetCharacterRace(Races _characterRace) => characterRace = _characterRace;

    [Server(Logging = LoggingType.Off)]
    public void SetTargetStatus(TargetStatus _targetStatus) => targetStatus = _targetStatus;

    [Server(Logging = LoggingType.Off)]
    public void SetCombatStatus(CombatStatus _combatStatus) => combatStatus = _combatStatus;

    [Server(Logging = LoggingType.Off)]
    public void SetActionState(ActionState _actionState) => actionState = _actionState;

    [Server(Logging = LoggingType.Off)]
    public void SetLevel(int _level) => level = _level;

    [Server(Logging = LoggingType.Off)]
    public void SetMaxHealth(float _maxHealth) => maxHealth = _maxHealth;

    [Server(Logging = LoggingType.Off)]
    public void SetCurrentHealth(float _currentHealth) => currentHealth = _currentHealth;

    [Server(Logging = LoggingType.Off)]
    public void SetMaxMana(float _maxMana) => maxMana = _maxMana;

    [Server(Logging = LoggingType.Off)]
    public void SetCurrentMana(float _currentMana) => currentMana = _currentMana;

    [Server(Logging = LoggingType.Off)]
    public void SetArmor(int _armor) => armor = _armor;

    [Server(Logging = LoggingType.Off)]
    public void SetSpeed(float _speed) => speed = _speed;

    [Server(Logging = LoggingType.Off)]
    public void SetMinAutoAttackDamage(int _minAutoAttackDamage) => minAutoAttackDamage = _minAutoAttackDamage;

    [Server(Logging = LoggingType.Off)]
    public void SetMaxAutoAttackDamage(int _maxAutoAttackDamage) => maxAutoAttackDamage = _maxAutoAttackDamage;

    [Server(Logging = LoggingType.Off)]
    public void SetAutoAttackRange(float _autoAttackRange) => autoAttackRange = _autoAttackRange;

    [Server(Logging = LoggingType.Off)]
    public void SetAutoAttackCooldown(float _autoAttackCooldown) => autoAttackCooldown = _autoAttackCooldown;

    [Server(Logging = LoggingType.Off)]
    public void SetAutoAttackTimer(float _autoAttackTimer) => autoAttackTimer = _autoAttackTimer;

    [Server(Logging = LoggingType.Off)]
    public void SetLocation(Vector3 _location) => location = _location;

    [Server(Logging = LoggingType.Off)]
    public void SetScale(Vector3 _scale) => scale = _scale;

    [Server(Logging = LoggingType.Off)]
    public void SetRotation(Quaternion _rotation) => rotation = _rotation;

    [Server(Logging = LoggingType.Off)]
    public void SetCurrentTarget(GameObject _currentTarget) => currentTarget = _currentTarget;

    [Server(Logging = LoggingType.Off)]
    public void SetWeaknesses(List<DamageTypes> _weaknesses) => weaknesses = _weaknesses;

    [Server(Logging = LoggingType.Off)]
    public void SetResistances(List<DamageTypes> _resistances) => resistances = _resistances;

    [Server(Logging = LoggingType.Off)]
    public void SetImmunities(List<DamageTypes> _immunities) => immunities = _immunities;

    [Server(Logging = LoggingType.Off)]
    public void SetAggroList(List<ICombatable> _aggroList) => aggroList = _aggroList;
}
