using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using UnityEngine;
using FishNet.Connection;
using System.Collections.Generic;

public class SceneController : NetworkBehaviour {
    public static SceneController instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();
        StartServer();
    }

    [Server(Logging = LoggingType.Off)]
    private void StartServer() {
        List<string> scenes = new List<string> { "Server", "Game" };
        SceneLoadData load = new SceneLoadData(scenes);
        load.Options.AutomaticallyUnload = false;
        load.ReplaceScenes = ReplaceOption.All;
        SceneManager.LoadConnectionScenes(load);
        scenes = new List<string> { "API" };
        load = new SceneLoadData(scenes);
        load.ReplaceScenes = ReplaceOption.None;
        SceneManager.LoadGlobalScenes(load);
        gameObject.GetComponent<NetworkObject>();
    }

    [Server(Logging = LoggingType.Off)]
    public void StartClient(NetworkConnection sender) {
        
    }

    [Server(Logging = LoggingType.Off)]
    public void LogIntoGame(NetworkConnection sender) {
        StartCoroutine(LogInSequence(sender));
    }

    [Server(Logging = LoggingType.Off)]
    private IEnumerator LogInSequence(NetworkConnection sender) {
        LoadGame(sender);
        //yield return new WaitForSeconds(1f);
        yield return null;
    }

    [Server(Logging = LoggingType.Off)]
    public void LoadGame(NetworkConnection sender) {
        // Load Game
        SceneLoadData load = new SceneLoadData("Game");
        load.ReplaceScenes = ReplaceOption.None;
        load.PreferredActiveScene.Client = load.SceneLookupDatas[0];
        load.PreferredActiveScene.Server = load.SceneLookupDatas[0];
        SceneManager.LoadConnectionScenes(sender, load);
    }
}
