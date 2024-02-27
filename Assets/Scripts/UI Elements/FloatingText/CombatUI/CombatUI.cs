using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ContentSizeFitter))]
public class CombatUI : UIElement {
    public GameObject combatTextPrefab;
    public Transform worldObjectTransform;
    private Camera playerCamera;

    private void Awake() {
        playerCamera = FindObjectOfType<Camera>();
    }

    // Method to be called by the controller to update position
    public void UpdatePosition() {
        if (worldObjectTransform != null) {
            Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldObjectTransform.position);
            transform.position = screenPosition;
        }
    }

    public void AddCombatText(string message) {
        GameObject newCombatTextObj = Instantiate(combatTextPrefab, transform);
        CombatText newCombatText = newCombatTextObj.GetComponent<CombatText>();

        // Set the text and initial position
        Debug.Log("Test 4");
        newCombatText.Initialize(message);

        // Set color based on the message
        newCombatText.SetColor(message.StartsWith("-") ? Color.white : Color.green);

        // Start the floating and fading behaviour
        newCombatText.FloatAndFade();
    }
}