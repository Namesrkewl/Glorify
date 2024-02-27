using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionBarButton : ActionButton {
    private TextMeshProUGUI text;

    protected override void Awake() {
        base.Awake();
        text = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }

    public void UpdateActionKeycode(string input) {
        text.text = input;
    }
}