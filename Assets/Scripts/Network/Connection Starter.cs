using FishNet;
using System.Collections;
using UnityEngine;

public enum ConnectionType {
    Server,
    Client,
    Host
}

public class ConnectionStarter : MonoBehaviour {
    public static ConnectionStarter instance;
    private bool connected;
    private bool connecting;
    public ConnectionType connectionType;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            connected = false;
            connecting = false;
            StartCoroutine(Connect());
        }
    }

    private void Update() {
        if (InstanceFinder.NetworkManager.IsOffline) {
            connected = false;
        } else {
            if (!connected) {
                Debug.Log("Connected!");
            }
            connected = true;
        }
        if (connected == false && !connecting) {
            StartCoroutine(Connect());
        }
        
        
    }

    private IEnumerator Connect() {
        connecting = true;
        if (InstanceFinder.NetworkManager.IsOffline) {
            Debug.Log("Server Connection Lost! Attempting to Reconnect...");
            if (connectionType != ConnectionType.Client) {
                InstanceFinder.NetworkManager.ServerManager.StartConnection();
            } else {
                InstanceFinder.NetworkManager.ClientManager.StartConnection();
            }
            yield return new WaitForSeconds(1);
            if (connectionType == ConnectionType.Host) {
                InstanceFinder.NetworkManager.ClientManager.StartConnection();
            }
        }
        connecting = false;
    }
}
