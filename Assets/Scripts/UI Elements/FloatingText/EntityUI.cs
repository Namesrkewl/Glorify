using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUI : UIElement {
    public Transform worldObjectTransform; // Reference to the actual entity in the world
    public CombatUI combatUI;
    public InformationUI informationUI;
    public NameplatesUI nameplatesUI;
    private Camera playerCamera;

    private void Awake() {
        playerCamera = FindObjectOfType<Camera>();
    }

    private void Update() {
        // Update the visibility and position for both UI elements
        UpdateUIVisibility(combatUI);
        UpdateUIVisibility(informationUI);
    }

    public void CreateCombatText(string message) {
        combatUI.worldObjectTransform = worldObjectTransform;
        combatUI.AddCombatText(message);
    }

    public void CreateInformationText(ITargetable entity) {
        informationUI.worldObjectTransform = worldObjectTransform;
        informationUI.AddInformationText(entity);
    }

    // Generalized update visibility method for any UI element
    private void UpdateUIVisibility(UIElement uiElement) {
        if (worldObjectTransform != null && uiElement != null) {

            Vector3 cameraRelative = playerCamera.transform.InverseTransformPoint(worldObjectTransform.position);

            if (cameraRelative.z > 0) {
                // The entity is in front of the camera
                if (uiElement is InformationUI informationUI) {
                    informationUI.UpdatePosition(); // Update its position
                } else if (uiElement is CombatUI combatUI) {
                    combatUI.UpdatePosition(); // Update its position
                }
                uiElement.gameObject.SetActive(true);
            } else {
                // The entity is behind the camera
                uiElement.gameObject.SetActive(false);
            }
        }
    }
}
