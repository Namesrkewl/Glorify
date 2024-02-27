using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class API : NetworkBehaviour {

    public static API instance;

    private void Awake() {
        instance = this;
        Debug.Log("API Loaded.");
    }

    // This script is an API that connects the Client to the Server.

    #region Client Connections
    public override void OnStartClient() {
        base.OnStartClient();
        StartClient();
    }

    public override void OnStopClient() {
        Debug.Log("Connection Stopped!");
        if (Client.instance != null) {
            if (!Client.instance.mainMenu.activeSelf) {
                Client.instance.mainMenu.SetActive(true);
            }
        }
        Debug.Log(UnityEngine.SceneManagement.SceneManager.sceneCount);
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++) {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            //Debug.Log($"{scene.name} found.");
            if (!IsServerInitialized) {
                if (scene.name != "Client" && scene.name != "API") {
                    //Debug.Log($"Unloading {scene.name}!");
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                }
            }
        }
    }

    public void Login(string name) {
        Debug.Log($"Processing Login...");
        ProcessLogin(name);
    }

    [TargetRpc] 
    private void CompleteLogin(NetworkConnection conn, int ID) {
        if (ID > 0) {
            Debug.Log("Login Successful!");
            Client.instance.SetID(ID);
            if (Client.instance.mainMenu.activeSelf) {
                Client.instance.mainMenu.SetActive(false);
                Debug.Log("Disabled the main menu!");
            }
            LoginPlayer();
        } else {
            Debug.Log("Login Unsuccessful.");
        }
    }

    #endregion

    #region Server Connections
    [ServerRpc(RequireOwnership = false)]
    private void StartClient(NetworkConnection sender = null) {
        SceneController.instance.StartClient(sender);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ProcessLogin(string name, NetworkConnection sender = null) {
        Player player = Database.instance.GetPlayer(Database.instance.GetPlayerID(name));
        Debug.Log($"Current Player available: {player != null}");
        Debug.Log(Database.instance.GetPlayerID(name));
        if (player == null) {
            player = PlayerManager.instance.CreatePlayer(name);
        }
        Debug.Log("We Returned To Process Login.");
        Debug.Log(sender);
        Debug.Log(LocalConnection);
        CompleteLogin(sender, player.GetID());
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoginPlayer(NetworkConnection sender = null) {
        SceneController.instance.LogIntoGame(sender);
    }
    #endregion
}
