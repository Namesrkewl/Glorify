using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class GameManager : NetworkBehaviour {
    #region Start Client

    public static GameManager instance;
    public List<PlayerClass> playerClasses = new List<PlayerClass>();

    #endregion

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            LoadAllPlayerClasses();
            UpdateAllClasses();
            // Get the AudioSource component attached to this GameObject
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();
        // Load the audio clip
        AudioClip clip = Resources.Load<AudioClip>("Media/Big Fantasy RPG Music Bundle/Market/Market_FULL_TRACK");
        if (clip != null) {
            API.instance.audioSource.clip = clip;
            API.instance.audioSource.Play();
        } else {
            Debug.LogError("Failed to load audio clip.");
        }
    }

    private void LoadAllPlayerClasses() {
        // Assuming you have a folder named "PlayerClasses" in Resources
        PlayerClass[] loadedClasses = Resources.LoadAll<PlayerClass>("Player Classes");
        foreach (var playerClass in loadedClasses) {
            playerClasses.Add(playerClass);
        }
    }

    public void UpdateAllClasses() {
        foreach (var playerClass in playerClasses) {
            playerClass.UpdateClass();
        }
    }
}