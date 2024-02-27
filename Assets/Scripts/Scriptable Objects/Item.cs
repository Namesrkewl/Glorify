using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject, Identifiable {
    public new string name;
    [System.NonSerialized] public Sprite Icon;
    public ItemType Type;
    private int itemID;

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

    public int GetID() {
        return itemID;
    }

    public void SetID(int input) {
        itemID = input;
    }
}