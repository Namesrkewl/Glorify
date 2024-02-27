using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using UnityEngine;

public class Spawner : NetworkBehaviour {
    public GameObject playerPrefab;

    public override void OnStartClient() {
        base.OnStartClient();
        Debug.Log("Spawning Player!");
        SpawnPlayer(Client.instance.GetID());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer(int ID, NetworkConnection sender = null) {
        Player player = Database.instance.GetPlayer(ID);
        GameObject playerObject = Instantiate(playerPrefab, player.location, player.rotation);
        playerObject.transform.localScale = player.scale;
        Debug.Log($"Spawning {player.name}!");
        Spawn(playerObject, sender);
    }
}
