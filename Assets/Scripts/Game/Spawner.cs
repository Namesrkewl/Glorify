using FishNet.Connection;
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
        if (player.networkObject == null) {
            NetworkObject playerObject = Instantiate(playerPrefab, new Vector3(2700, 12, 2200), Quaternion.identity).GetComponent<NetworkObject>();
            player.networkObject = playerObject;
        } else {
            NetworkObject playerObject = Instantiate(player.networkObject, player.networkObject.transform.position, player.networkObject.transform.rotation);
            player.networkObject = playerObject;
        }
        Debug.Log($"Spawning {player.name} with the ID {player.ID}!");
        Spawn(player.networkObject, sender);
    }
}
