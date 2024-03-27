using UnityEngine;
using MoonSharp.Interpreter;
using UnityEditor;
using System;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Managing.Logging;

[MoonSharpUserData, CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
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
    [SerializeField] private TextAsset luaSpellScript;
    public List<DamageSchool> damageSchoolList;
    public List<GameObject> gameObjects;
    // Other spell properties like damage, duration, etc., can be added here.

    public override void ExecuteAction(PlayerBehaviour playerBehaviour) {
        //playerBehaviour.HandleSpell(this);
    }

    [Server(Logging = LoggingType.Off)]
    public virtual void Cast(Character caster, Character target) {
        if (luaSpellScript != null) {
            Script script = new Script();

            // Register the logging function
            script.Globals["log"] = (Action<object>)LuaLogger.Log;

            // Expose objects to Lua
            script.Globals["spell"] = this;
            script.Globals["caster"] = caster;
            script.Globals["target"] = target;
            script.Globals["combatManager"] = CombatManager.instance;

            // Execute the Lua script
            script.DoString(luaSpellScript.text);

            // Retrieve the 'cast' function from the Lua script
            DynValue luaCastFunction = script.Globals.Get("Cast");

            // Check if the function is found and callable
            if (luaCastFunction.Type == DataType.Function) {
                // Call the 'cast' function with 'caster' and 'target' as arguments
                script.Call(luaCastFunction, caster, target);
            } else {
                Debug.LogError("The 'cast' function was not found in the Lua script.");
            }
        }
    }

    public virtual void RemoveEffect(Character character) {

    }

    public virtual void UpdateDescription() {

    }
}
