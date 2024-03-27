using UnityEngine;
using MoonSharp.Interpreter;

public class LuaEnvironmentSetup : MonoBehaviour {
    // This method will be automatically called when the game starts
    void Awake() {
        SetupMoonSharpEnvironment();
    }

    private void SetupMoonSharpEnvironment() {
        // Register all the types you'll use in your Lua scripts
        UserData.RegisterType<CombatManager>();
        UserData.RegisterType<Spell>();
        UserData.RegisterType<Character>();
        UserData.RegisterType<Player>();
        UserData.RegisterType<NPC>();

        // Additional setup can be done here (e.g., global variables, custom converters)
    }
}