using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item"), Serializable]
public class Item : ScriptableObject, Identifiable {
    public new string name;
    [System.NonSerialized] public Sprite Icon;
    public ItemType Type;
    public Key key;

    public enum ItemType {
        Armor,
        Consumable,
        Container,
        Gem,
        Key,
        Miscellaneous,
        Money,
        Reagent,
        Recipe,
        Projectile,
        Quest,
        Quiver,
        TradeGoods,
        Weapon
    }

    void Start() {
        // Initialization or any startup logic goes here
    }

    void Update() {
        // Any update logic for the item goes here
    }

    // Add additional methods to handle item behaviours as needed

    public Key GetKey() {
        return key;
    }

    public void SetKey(Key _key) {
        key = _key;
    }
}