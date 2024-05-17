using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;

public class TreeSyncList : NetworkBehaviour
{
    public GameObject wholeTree;
    public GameObject firTreeDivided;
    public GameObject stump;
    public GameObject topAndBottomHalf;
    public GameObject topHalf;
    public GameObject lowerHalf;

    public readonly SyncList<GameObject> _myCollection = new SyncList<GameObject>();

    // Create SyncList of GameObjects called _myCollection and include all the GameObjects declared in this class
    private void Start()
    {
        _myCollection.Add(wholeTree);
        _myCollection.Add(firTreeDivided);
        _myCollection.Add(stump);
        _myCollection.Add(topAndBottomHalf);
        _myCollection.Add(topHalf);
        _myCollection.Add(lowerHalf);
    }

    private void Awake()
    {
        _myCollection.OnChange += _myCollection_OnChange;
    }

    private void Update()
    {

    }
    private void _myCollection_OnChange(SyncListOperation op, int index, GameObject oldItem, GameObject newItem, bool asServer)
    {
        switch (op)
        {
            case SyncListOperation.Add:
                break;
            case SyncListOperation.RemoveAt:
                break;
            case SyncListOperation.Insert:
                break;
            case SyncListOperation.Set:
                break;
            case SyncListOperation.Clear:
                break;
            case SyncListOperation.Complete:
                break;
        }
    }
}