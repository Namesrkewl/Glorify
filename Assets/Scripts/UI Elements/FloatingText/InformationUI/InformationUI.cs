using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ContentSizeFitter))]
public class InformationUI : UIElement {
    public GameObject InformationTextPrefab;
    public Transform worldObjectTransform;
    ITargetable _entity;
    public Vector3 offset;
    private Camera playerCamera;

    private void Awake() {
        playerCamera = FindObjectOfType<Camera>();
    }

    // Method to be called by the controller to update position
    public void UpdatePosition() {
        if (worldObjectTransform != null) {
            float entityHeight = GetEntityHeight(_entity); // This will need to be implemented
            offset = new Vector3(0, entityHeight, 0);
            Vector3 worldPosition = worldObjectTransform.position + offset;
            Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

            transform.position = screenPosition;
        }
    }

    public void AddInformationText(ITargetable entity) {
        _entity = entity;
        GameObject newInformationTextObj = Instantiate(InformationTextPrefab, transform);
        InformationText newInformationText = newInformationTextObj.GetComponent<InformationText>();
        // Set the text and initial position
        newInformationText.Initialize(entity);
    }

    private float GetEntityHeight(ITargetable entity) {
        // This is an example. Replace it with your actual method of determining height.
        GameObject entityObject = entity.GetTargetObject();
        Collider collider = entityObject.GetComponent<Collider>();
        if (collider != null) {
            return collider.bounds.size.y;
        }
        return 1.0f; // Default height if no collider is found
    }
}