using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton
    
    public static Inventory instance;
    
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than one instance of Inventory found! Destroying one.");
            Destroy(gameObject);
            return;
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    private List<Item> items;
    public int Capacity;
    void Start() {
        items = new List<Item>(Capacity);
    }

    public bool AddItem(Item item) {
        if (items.Count < Capacity) {
            items.Add(item);
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
            return true; // Item added successfully
        } else {
            return false; // Inventory full
        }
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public Item GetItem(int index) {
        if (index >= 0 && index < items.Count) {
            return items[index];
        }
        return null; // No item found at the given index
    }

    // Additional methods for inventory management can be added here
}