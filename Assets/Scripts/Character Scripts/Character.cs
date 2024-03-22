using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class Character : Identifiable {
    public string name;
    public int ID;
    public TargetType targetType;
    public Races characterRace;
    public TargetStatus targetStatus;
    public CombatStatus combatStatus;
    public ActionState actionState;
    public int level;
    public float maxHealth;
    public float currentHealth;
    public float maxMana;
    public float currentMana;
    public int armor;
    public int minAutoAttackDamage;
    public int maxAutoAttackDamage;
    public float autoAttackRange;
    public float speed;
    public float autoAttackCooldown;
    public float autoAttackTimer = 0f;
    public NetworkObject networkObject;
    public GameObject currentTarget;
    public List<DamageTypes> weaknesses;
    public List<DamageTypes> resistances;
    public List<DamageTypes> immunities;
    public List<GameObject> aggroList = new List<GameObject>();

    public Key GetKey() {
        Key key = new Key { ID = ID, name = name };
        return key;
    }

    public virtual ITargetable GetBehaviour() {
        return null;
    }

    [Server(Logging = LoggingType.Off)]
    public virtual void Sync() {
        
    }

    [Client(Logging = LoggingType.Off)]
    public virtual void SetInformationText(InformationText info) {

    }
}
