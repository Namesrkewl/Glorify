using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class ResourceNode : NetworkBehaviour
{
    public HarvestObject harvestObject;
    [HideInInspector]
    public readonly SyncVar<int> harvestAmount = new SyncVar<int>();
    [HideInInspector]
    public readonly SyncVar<int> health = new SyncVar<int>();
    //public ItemSO itemSO;
    [HideInInspector]
    //[SyncVar] public NetItem netItem;

    public readonly SyncList<GameObject> _myCollection = new SyncList<GameObject>();

    private void Awake()
    {

        _myCollection.OnChange += _myCollection_OnChange;
    }
    public override void OnStartServer()
    {

        harvestAmount.Value = Random.Range(harvestObject.minAmount, harvestObject.maxAmount);
        health.Value = harvestObject.defaultHealth;
        base.OnStartServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (IsServerInitialized)
        {
            if (harvestObject.harvestType == HarvestObject.HarvestType.Tree)
            {
                _myCollection.Add(gameObject);
            }
        }
    }

    private void _myCollection_OnChange(SyncListOperation op, int index, GameObject oldItem, GameObject newItem, bool asServer)
    {
        switch (op)
        {
            /* An object was added to the list. Index
             * will be where it was added, which will be the end
             * of the list, while newItem is the value added. */
            case SyncListOperation.Add:
                Debug.Log("Added " + newItem.name + " to the list.");
                break;
            /* An object was removed from the list. Index
             * is from where the object was removed. oldItem
             * will contain the removed item. */
            case SyncListOperation.RemoveAt:
                Debug.Log("Removed " + oldItem.name + " from the list.");
                break;
            /* An object was inserted into the list. Index
             * is where the object was inserted. newItem
             * contains the item inserted. */
            case SyncListOperation.Insert:
                Debug.Log("Inserted " + newItem.name + " to the list.");
                break;
            /*An object replaced another. Index
             * is where the object was replaced. oldItem
             * is the item that was replaced, while
             * newItem is the item which now has it's place. */
            case SyncListOperation.Set:
                Debug.Log("Set " + newItem.name + " to the list.");
                break;
            /* All objects have been cleared. Index, oldValue,
             * and newValue are default. */
            case SyncListOperation.Clear:
                Debug.Log("Cleared the list.");
                break;
            /* When complete calls all changes have been
             * made to the collection. You may use this
             * to refresh information in relation to
             * the list changes, rather than doing so
             * after every entry change. Like Clear
             * Index, oldItem, and newItem are all default. */
            case SyncListOperation.Complete:
                Debug.Log("Completed the list.");
                break;

        }
    }

    //public void Harvest(NetworkConnection conn, Inventory playerInventory, bool fullyHarvest = false)
    [ServerRpc(RequireOwnership = false)]
    public void Harvest(NetworkConnection conn, bool fullyHarvest = true)
    {
        Debug.Log("Harvesting");
        HarvestClient(conn);
        //HarvestClient(conn, playerInventory);
        if (fullyHarvest == true)
        {
            base.Despawn(DespawnType.Pool);
        }
        //Debug.Log("Destroyed");
    }

    [TargetRpc]
    //public void HarvestClient(NetworkConnection conn, Inventory playerInventory)
    public void HarvestClient(NetworkConnection conn)
    {
        Debug.Log(harvestAmount + harvestObject.objectName + " harvested");
        //netItem = new NetItem(itemSO.Id, itemSO.MaxStack, harvestAmount);


        /********** Add to inventory to Glorify**********/
        Debug.Log("Add to inventory to Glorify");

        ///playerInventory.RequestAddItemServerRpc(netItem, conn);


        //NetItem netItem = new NetItem(harvestObject.name, harvestAmount);
        //playerInventory.TryAddItem
    }

    // Update is called once per frame
    void Update()
    {

    }
}
