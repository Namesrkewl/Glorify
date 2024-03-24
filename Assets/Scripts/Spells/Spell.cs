using UnityEngine;
using MoonSharp.Interpreter;
using FishNet.Object;

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
    public DamageType damageType;
    public DamageSchool damageSchool;
    public TextAsset luaSpellScript;
    // Other spell properties like damage, duration, etc., can be added here.

    public override void ExecuteAction(PlayerBehaviour playerBehaviour) {
        //playerBehaviour.HandleSpell(this);
    }

    public virtual void Cast(Character caster, Character target) {
        if (luaSpellScript != null) {
            Script script = new Script();
            // Expose C# objects to Lua
            script.Globals["caster"] = caster;
            script.Globals["target"] = target;
            script.Globals["combatManager"] = CombatManager.instance;

            // Execute the Lua script.
            script.DoString(luaSpellScript.text);
        }
    }

    public virtual void RemoveEffect(Character character) {

    }

    public virtual void UpdateDescription() {

    }
}
