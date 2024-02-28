using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity {
    Normal,
    Rare,
    Elite,
    Boss
}

[Serializable]
public class NPC : Character {
    public float maxAttackRange = 50f; 
    public float aggroRange = 10f; // Base range for initiating combat
    public Vector3 lastAttackPosition; // Last position where the NPC attacked the player
    public Vector3 originalPosition;
    public Rarity rarity;
    public int wealth;
    public float experience;
    public List<Item> dropTable;

    public NPC() {
        key = new Key();
    }
}