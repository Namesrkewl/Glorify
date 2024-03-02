using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class API : NetworkBehaviour {

    public static API instance;
    public Key clientKey;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            Debug.Log("API Loaded.");
        }
    }

    // This script is an API that connects the Client to the Server.

    #region Client Connections
    public override void OnStartClient() {
        base.OnStartClient();
        StartClient();
    }

    public override void OnStopClient() {
        Debug.Log("Connection Stopped!");
        if (Client.instance.mainMenu != null) {
            if (!Client.instance.mainMenu.activeSelf) {
                Client.instance.mainMenu.SetActive(true);
            }
        }
        if (ChatManager.instance != null) {
            ChatManager.instance.gameObject.SetActive(false);
        }
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

    public void Login(string name, string pass) {
        Debug.Log($"Processing Login...");
        ProcessLogin(name, pass);
    }

    [TargetRpc] 
    public void CompleteLogin(NetworkConnection conn, Key key) {
        Debug.Log("Login Successful!");
        clientKey = key;
        if (Client.instance.mainMenu.activeSelf) {
            Client.instance.mainMenu.SetActive(false);
            Debug.Log("Disabled the main menu!");
        }
        LoginPlayer();
    }

    #endregion

    #region Server Connections
    [ServerRpc(RequireOwnership = false)]
    private void StartClient(NetworkConnection sender = null) {
        SceneController.instance.StartClient(sender);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ProcessLogin(string name, string pass, NetworkConnection sender = null) {
        Credentials credentials = new Credentials();
        credentials.username = name;
        credentials.password = pass;
        Key key = Database.instance.GetUser(credentials);
        Player player = Database.instance.GetPlayer(key) ?? null;
        Debug.Log($"Current Player available: {player != null}");
        if (player == null && !Database.instance.CheckAvailability(credentials)) {
            Debug.Log("Username taken!");
            return;
        } else if (player == null) {
            PlayerManager.instance.CreatePlayer(name, credentials, sender);
        } else {
            CompleteLogin(sender, player.key);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoginPlayer(NetworkConnection sender = null) {
        SceneController.instance.LogIntoGame(sender);
    }
    #endregion
}
