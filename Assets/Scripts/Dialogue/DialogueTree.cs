using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Tree", menuName = "Dialogue/Dialogue Tree")]
public class DialogueTree : ScriptableObject {
    public new string name;
    public List<Dialogue> dialogue;

    public void InitiateDialogue() {
        // Depending on flags on the player, call a specific index of dialogue
        DialogueManager.instance.ShowDialogue(this);
    }
}
