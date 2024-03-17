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
    [SerializeField] private float maxAttackRange = 50f;
    [SerializeField] private float aggroRange = 10f; // Base range for initiating combat
    [SerializeField] private Vector3 lastAttackPosition; // Last position where the NPC attacked the player
    [SerializeField] private Rarity rarity;
    [SerializeField] private int wealth;
    [SerializeField] private float experience;
    [SerializeField] private List<Item> dropTable;

    // Getters
    public float GetMaxAttackRange() => maxAttackRange;
    public float GetAggroRange() => aggroRange;
    public Vector3 GetLastAttackPosition() => lastAttackPosition;
    public Rarity GetRarity() => rarity;
    public int GetWealth() => wealth;
    public float GetExperience() => experience;
    public List<Item> GetDropTable() => dropTable;

    // Setters with Server attribute
    [Server(Logging = LoggingType.Off)]
    public void SetMaxAttackRange(float _maxAttackRange) => maxAttackRange = _maxAttackRange;

    [Server(Logging = LoggingType.Off)]
    public void SetAggroRange(float _aggroRange) => aggroRange = _aggroRange;

    [Server(Logging = LoggingType.Off)]
    public void SetLastAttackPosition(Vector3 _lastAttackPosition) => lastAttackPosition = _lastAttackPosition;

    [Server(Logging = LoggingType.Off)]
    public void SetRarity(Rarity _rarity) => rarity = _rarity;

    [Server(Logging = LoggingType.Off)]
    public void SetWealth(int _wealth) => wealth = _wealth;

    [Server(Logging = LoggingType.Off)]
    public void SetExperience(float _experience) => experience = _experience;

    [Server(Logging = LoggingType.Off)]
    public void SetDropTable(List<Item> _dropTable) => dropTable = _dropTable;
}