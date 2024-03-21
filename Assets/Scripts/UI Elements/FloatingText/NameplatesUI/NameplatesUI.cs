using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter))]
public class NameplatesUI : MonoBehaviour {

    void Awake() {

    }

    public void AddNameplateText(FloatingText text) {
        text.transform.SetParent(transform, false);
    }
}
