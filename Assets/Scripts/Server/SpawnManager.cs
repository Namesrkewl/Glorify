using FishNet.Object;
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

}
