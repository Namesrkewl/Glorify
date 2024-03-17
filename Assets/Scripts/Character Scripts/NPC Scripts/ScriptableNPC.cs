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
        if (npc.GetName() != name) {
            npc.SetName(name);
        } else if (npc.GetID() != ID) {
            npc.SetID(ID);
        }        
    }
}

