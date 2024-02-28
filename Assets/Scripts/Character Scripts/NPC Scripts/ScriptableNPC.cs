using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NPC", menuName = "Character/NPC")]
public class ScriptableNPC : ScriptableObject {
    public NPC npc;
    public Image image;
    public new string name;
    public int ID;
    public Key key;
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

    private void Awake() {
        key = new Key();
        npc = new NPC();
        npc.key = key;
    }

    private void OnValidate() {
        key.name = name;
        key.ID = ID;
        npc.image = image;       
        npc.targetType = targetType;
        npc.characterRace = characterRace;
        npc.targetStatus = targetStatus;
        npc.combatStatus = combatStatus;
        npc.level = level;
        npc.maxHealth = maxHealth;
        npc.maxMana = maxMana;
        npc.currentMana = currentMana;
        npc.armor = armor;
        npc.speed = speed;
        npc.autoAttackCooldown = autoAttackCooldown;
        npc.autoAttackRange = autoAttackRange;
        npc.location = location;
        npc.scale = scale;
        npc.rotation = rotation;
        npc.weaknesses = weaknesses;
        npc.resistances = resistances;
        npc.immunities = immunities;
        npc.rarity = rarity;
        npc.wealth = wealth;
        npc.experience = experience;
        npc.dropTable = dropTable;
    }
}

