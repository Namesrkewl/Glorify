using UnityEngine;

public class FloatingTextTester : MonoBehaviour {
    public FloatingTextManager floatingTextManager;
    public EntityUI entityUI; // Reference to the CombatUI

    void Update() {
        // Check for Return key to simulate damage
        if (Input.GetKeyDown(KeyCode.Return)) {
            CreateCombatText("-10");
        }

        // Check for Backspace key to simulate healing
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            CreateCombatText("+10");
        }
    }

    private void CreateCombatText(string message) {
        Debug.Log("Test");
        floatingTextManager.CreateCombatText(entityUI, message);
        // Optionally, destroy the simulated entity after a short delay
        //Destroy(entity, 1.0f);
    }
}