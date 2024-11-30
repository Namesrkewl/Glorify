using System.Collections.Generic;
using UnityEngine;

public class Inventory2 : MonoBehaviour
{

    #region Singleton

    public static Inventory2 instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found! Destroying one.");
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    private List<Item2> items;
    public int Capacity;
    void Start()
    {
        items = new List<Item2>(Capacity);
    }

    public bool AddItem(Item2 item)
    {
        if (items.Count < Capacity)
        {
            items.Add(item);
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
            return true; // Item added successfully
        }
        else
        {
            return false; // Inventory full
        }
    }

    public void RemoveItem(Item2 item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public Item2 GetItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null; // No item found at the given index
    }

    // Additional methods for inventory management can be added here
}