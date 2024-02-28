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

public class NPC : Character {
    public Rarity rarity;
    public int wealth;
    public float experience;
    public List<Item> dropTable;

    public NPC() {
        key = new Key();
        Database.instance.npcs.Add(this);
    }

    // Other properties and methods specific to NPCs
    private void Awake() {
        Debug.Log("The NPC is Awake!");
    }

    private void OnValidate() {
        Debug.Log("The NPC was validated!");
    }
}