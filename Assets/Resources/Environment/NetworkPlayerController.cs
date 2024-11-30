using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using FishNet.Object;

public class NetworkPlayerController : NetworkBehaviour
{
    // This goes on each of your players, you probably have a script like this named differently
    public Camera localCamera;
    /*
    private void Awake()
    {
        // Get your camera based off your player script's transform in hierarchy
        localCamera = GetComponentInChildren<Camera>();
    }
    */
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;
        // Set your client's version of PlayerManager's player if you're the owner
        PlayerManager.instance.SetPlayer(this);
        localCamera = GetComponentInChildren<Camera>();
    }

    // Other script stuff goes below
}
