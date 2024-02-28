using System.Collections;
using UnityEngine;


public class Spell : PlayerAction, Identifiable {
    public SpellType type;
    public enum SpellType {
        Damaging,
        Healing,
        Movement,
        Passive,
        Support,
    }
    public TargetType targetType;
    public enum TargetType {
        Ally,
        Any,
        Area,
        Aura,
        Enemy,
        Self
    }
    public School school;
    public enum School {
        Magic,
        Physical,
        Ranged
    }
    public float cooldown;
    public float duration;
    public float castTime;
    public int manaCost;
    public int levelRequired;
    public int damage;
    public int range;
    public bool needsLineOfSight;
    public bool isGlobalCooldown;
    public DamageTypes damageType;
    private int spellID;
    // Other spell properties like damage, duration, etc., can be added here.

    public override void ExecuteAction(PlayerBehaviour playerBehaviour) {
        //playerBehaviour.HandleSpell(this);
    }

    public virtual IEnumerator Cast(ICastable playerBehaviour, ICastable targetBehavior) {
        yield return null;
    }

    public virtual void RemoveEffect(Character character) {

    }

    public virtual void UpdateDescription() {

    }

    public virtual int GetID() {
        return spellID;
    }
    public virtual void SetID(int input) {
        spellID = input;
    }
}
