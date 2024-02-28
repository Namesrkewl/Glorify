using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NPC", menuName = "Character/NPC")]
public class ScriptableNPC : ScriptableObject {
    public NPC npc;
    public new string name;
    public int ID;

    private void OnValidate() {
        if (npc.key.name != name) {
            npc.key.name = name;
        } else if (npc.key.ID != ID) {
            npc.key.ID = ID;
        }        
    }
}

