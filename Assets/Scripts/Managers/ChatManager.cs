using UnityEngine;
using TMPro;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ChatManager: MonoBehaviour {
    public static ChatManager instance;
    public Transform chatHolder;
    public GameObject msgElement;
    public GameObject container;
    public TMP_InputField playerMessage;
    public PlayerControls playerControls;
    private bool tryingToFocus = false; // New flag to track focus intent

    private void Awake() {
        instance = this;
        container.SetActive(false);
    }

    private void OnEnable() {
        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageRecieved);
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageRecieved);
    }
    private void OnDisable() {
        if (InstanceFinder.ClientManager != null)
            InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageRecieved);
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageRecieved);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if (!playerMessage.isFocused && !tryingToFocus) {
                playerMessage.ActivateInputField();
                playerMessage.MoveTextEnd(false);
                tryingToFocus = true; // We're trying to focus the input field
            } else if (playerMessage.text != "" && (playerMessage.isFocused || tryingToFocus)) {
                SendMessage();
                playerMessage.text = "";
                EventSystem.current.SetSelectedGameObject(null); // Deselect input field
                tryingToFocus = false; // Reset flag
            } else if (playerMessage.text == "" && (playerMessage.isFocused || tryingToFocus)) {
                playerMessage.text = "";
                EventSystem.current.SetSelectedGameObject(null);
                tryingToFocus = false; // Reset flag here as well
            }
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            playerMessage.text = "";
            EventSystem.current.SetSelectedGameObject(null);
            tryingToFocus = false; // Reset flag here as well
        }
    }

    private void SendMessage() {
        Message msg = new Message() {
            username = API.instance.clientKey.name,
            message = playerMessage.text
        };

        playerMessage.text = "";

        if (InstanceFinder.IsServerStarted) {
            InstanceFinder.ServerManager.Broadcast(msg);
        }
        else if (InstanceFinder.IsClientStarted) {
            Debug.Log("Client up");
            InstanceFinder.ClientManager.Broadcast(msg);

        }
    }

    private void OnMessageRecieved(Message msg, Channel channel) {
        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        finalMessage.GetComponent<TextMeshProUGUI>().text = msg.username + ":  " + msg.message;
    }

    private void OnClientMessageRecieved(NetworkConnection networkConnection, Message msg, Channel channel) {
        Debug.Log("Server up");
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast {
        public string username;
        public string message;
    }
}