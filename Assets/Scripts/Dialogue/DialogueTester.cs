using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public DialogueTree dialogueTree;

    private void Update() {
        if (DialogueManager.instance.talking && Vector3.Distance(PlayerBehaviour.instance.transform.position, transform.position) > 5f) {
            DialogueManager.instance.CloseDialogue();
        }
    }

    private void OnMouseDown() {
        if (Vector3.Distance(PlayerBehaviour.instance.transform.position, transform.position) < 5f) {
            Talk();
        }
    }

    public void Talk() {
        if (dialogueTree != null) {
            dialogueTree.InitiateDialogue();
        }
    }
}
