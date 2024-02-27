using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Connection;

public class FloatingText : MonoBehaviour {
    protected TextMeshProUGUI textMesh;
    protected Vector3 upwardOffset = Vector3.zero;

    protected virtual void Awake() {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public virtual void Initialize(string text) {
        Debug.Log(textMesh);
        textMesh.text = text;
    }

    public virtual void Initialize(EntityBehaviour entity) {

    }

    protected virtual void Update() {
        transform.position = transform.position + upwardOffset;
    }
}