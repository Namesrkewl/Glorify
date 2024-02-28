using UnityEngine;
using FishNet.Object;

public class Server : NetworkBehaviour {
    public static Server instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void Update() {
        if (ConnectionStarter.instance.connectionType == ConnectionType.Host && !UnityEngine.SceneManagement.SceneManager.GetSceneByName("Client").isLoaded) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Client", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            Debug.Log("Client Loaded because you're host!");
        }
    }
}
