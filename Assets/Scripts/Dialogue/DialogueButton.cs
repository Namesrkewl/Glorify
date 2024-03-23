using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueButton : MonoBehaviour {
    public Dialogue dialogue;
    public TextMeshProUGUI textMesh;

    public void SelectOption() {
        DialogueManager.instance.ShowDialogue(dialogue);
    }
}
