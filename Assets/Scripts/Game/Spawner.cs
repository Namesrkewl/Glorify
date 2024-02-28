using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using UnityEngine;

public class Spawner : NetworkBehaviour {
    public GameObject playerPrefab;

    public override void OnStartClient() {
        base.OnStartClient();
        Debug.Log("Spawning Player!");
        SpawnPlayer(Client.instance.GetKey());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer(Key key, NetworkConnection sender = null) {
        Player player = Database.instance.GetPlayer(key);
        GameObject playerObject = Instantiate(playerPrefab, player.location, player.rotation);
        playerObject.transform.localScale = player.scale;
        Debug.Log($"Total Player Count: {Database.instance.players.Count}");
        Debug.Log($"Spawning {player.key.name} with the ID {player.key.ID}!");
        Spawn(playerObject, sender);
    }
}
