using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class API : NetworkBehaviour {

    public static API instance;
    public Key clientKey;
    public AudioSource audioSource;
    public ChatManager chatManager;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            chatManager = GetComponent<ChatManager>();
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
        if (Client.instance.mainMenu != null && !Client.instance.mainMenu.IsDestroyed()) {
            if (!Client.instance.mainMenu.activeSelf) {
                Client.instance.mainMenu.SetActive(true);
            }
        }
        if (ChatManager.instance.container != null && !ChatManager.instance.container.IsDestroyed()) {
            ChatManager.instance.container.SetActive(false);
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
        audioSource.Stop();
    }

    [Client(RequireOwnership = false)]
    public void Login(string name, string pass) {
        Debug.Log($"Processing Login...");
        ProcessLogin(name, pass);
    }

    [TargetRpc] 
    public void CompleteLogin(NetworkConnection conn, Key key) {
        Debug.Log("Login Successful!");
        clientKey = key;
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
        Player player = Database.instance.GetUser(credentials) ?? null;
        Debug.Log($"Current Player available: {player != null}");
        if (player == null && !Database.instance.CheckAvailability(credentials)) {
            Debug.Log("Username taken!");
            return;
        } else if (player == null) {
            PlayerManager.instance.CreatePlayer(name, credentials, sender);
        } else {
            NetworkConnection loggedInClient = Database.instance.GetClient(player);
            if (loggedInClient != null) {
                ServerManager.Clients.TryGetValue(loggedInClient.ClientId, out NetworkConnection target);
                if (target != null) {
                    Debug.Log("Kicking old client!");
                    loggedInClient.Kick(KickReason.Unset, loggingType: FishNet.Managing.Logging.LoggingType.Off);
                    Database.instance.RemoveClient(player);
                } else {
                    Database.instance.RemoveClient(player);
                }
            }
            Database.instance.AddClient(player, sender);
            CompleteLogin(sender, player.GetKey());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoginPlayer(NetworkConnection sender = null) {
        SceneController.instance.LogIntoGame(sender);
    }
    #endregion
}
