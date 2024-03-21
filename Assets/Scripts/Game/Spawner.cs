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
        if (player.gameObject == null) {
            GameObject playerObject = Instantiate(playerPrefab, new Vector3(2700, 12, 2200), Quaternion.identity);
            player.gameObject = playerObject;
        } else {
            GameObject playerObject = Instantiate(player.gameObject, player.gameObject.transform.position, player.gameObject.transform.rotation);
            player.gameObject = playerObject;
        }
        Debug.Log($"Spawning {player.name} with the ID {player.ID}!");
        Spawn(player.gameObject, sender);
    }
}
