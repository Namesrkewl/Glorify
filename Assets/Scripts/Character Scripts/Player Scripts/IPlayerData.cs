using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPlayerData {
    string Name { get; }
    int ID { get; }
    //Image Image { get; }
    TargetType TargetType { get; }
    Races CharacterRace { get; }
    TargetStatus TargetStatus { get; }
    CombatStatus CombatStatus { get; }
    ActionState ActionState { get; }
    int Level { get; }
    float MaxHealth { get; }
    float CurrentHealth { get; }
    float MaxMana { get; }
    float CurrentMana { get; }
    int Armor { get; }
    int MinAutoAttackDamage { get; }
    int MaxAutoAttackDamage { get; }
    float AutoAttackRange { get; }
    float Speed { get; }
    float AutoAttackCooldown { get; }
    GameObject GameObject { get; }
    List<DamageTypes> Weaknesses { get; }
    List<DamageTypes> Resistances { get; }
    List<DamageTypes> Immunities { get; }
    List<GameObject> AggroList { get; }

    // Player-specific properties
    PlayerClass PlayerClass { get; }
    Classes ClassEnum { get; }
    int Strength { get; }
    int Dexterity { get; }
    int Intelligence { get; }
    int CriticalStrike { get; }
    int Haste { get; }
    int Wisdom { get; }
    int Vitality { get; }
    int Leech { get; }
    int Avoidance { get; }
    int Constitution { get; }
    int Dodge { get; }
    int Block { get; }
    int Parry { get; }
    float MaxExperience { get; }
    float CurrentExperience { get; }
    bool IsSafe { get; }
    float RegenerationCooldownTimer { get; }
    List<Spell> Spells { get; }
    List<Spell> Passives { get; }

    public Key GetKey();
}