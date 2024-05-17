using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using V_AnimationSystem;
using UnityEngine;

public class GameAssets : NetworkBehaviour
{
    public GameObject gameFirTree;
    public Transform pfDamagePopup;
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null)
            {
                _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            }
            return _i;
        }
    }

    public readonly SyncList<GameObject> _myFirTreeCollection = new SyncList<GameObject>();

    public readonly SyncList<GameObject> _pfDamagePopupCollection = new SyncList<GameObject>();


    // Create SyncList of GameObjects called _myCollection and include all the GameObjects declared in this class
    private void Start()
    {
        _myFirTreeCollection.Add(gameFirTree);
    }

    private void Awake()
    {
        _myFirTreeCollection.OnChange += _myFirTreeCollection_OnChange;
        _pfDamagePopupCollection.OnChange += _pfDamagePopupCollection_OnChange;

    }

    private void Update()
    {

    }
    private void _myFirTreeCollection_OnChange(SyncListOperation op, int index, GameObject oldItem, GameObject newItem, bool asServer)
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

    private void _pfDamagePopupCollection_OnChange(SyncListOperation op, int index, GameObject oldItem, GameObject newItem, bool asServer)
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
