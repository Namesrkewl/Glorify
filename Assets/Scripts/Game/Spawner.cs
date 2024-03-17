using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using UnityEngine;

public class Spawner : NetworkBehaviour {
    public GameObject playerPrefab;

    public override void OnStartClient() {
        base.OnStartClient();
        SpawnPlayer(API.instance.clientKey);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer(Key key, NetworkConnection sender = null) {
        Player player = Database.instance.GetPlayer(key);
        GameObject playerObject = Instantiate(playerPrefab, player.GetLocation(), player.GetRotation());
        playerObject.transform.localScale = player.GetScale();
        Debug.Log($"Spawning {player.GetName()} with the ID {player.GetID()}!");
        Spawn(playerObject, sender);
    }
}
