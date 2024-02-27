using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "Character/NPC")]
public class NPC : Character, Identifiable {
    public Rarity rarity;
    public int wealth;
    public float experience;
    public List<Item> dropTable;
    private int npcID;

    public enum Rarity {
        Normal,
        Rare,
        Elite,
        Boss
    }

    // Other properties and methods specific to NPCs
    public int GetID() {
        return npcID;
    }

    public void SetID(int input) { 
        npcID = input;
    }

}