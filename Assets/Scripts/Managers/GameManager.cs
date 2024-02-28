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
