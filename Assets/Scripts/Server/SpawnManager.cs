using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkBehaviour {

    public static SpawnManager instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public GameObject playerPrefab;

    [Server(Logging = LoggingType.Off)]
    public void SpawnPlayer(Player player, NetworkConnection sender) {
        GameObject playerObject = Instantiate(playerPrefab, player.location, player.rotation);
        playerObject.transform.localScale = player.scale;
        Debug.Log($"Spawning {player.key.name}!");
        base.Spawn(playerObject, sender);
    }
}
