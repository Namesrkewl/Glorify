using UnityEngine;
using UnityEngine.UI;

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
}