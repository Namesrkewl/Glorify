using UnityEngine;
using TMPro;

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

    public virtual void Initialize(ITargetable entity) {

    }

    protected virtual void Update() {
        transform.position = transform.position + upwardOffset;
    }
}