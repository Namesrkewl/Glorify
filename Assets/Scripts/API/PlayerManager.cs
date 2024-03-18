using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing.Logging;
using System.Linq;
using UnityEditor;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using GameKit.Dependencies.Utilities;
using FishNet.Demo.AdditiveScenes;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager instance;
    private PlayerBehaviour playerBehaviour;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void CreatePlayer(string name, Credentials credentials, NetworkConnection sender) {
        Debug.Log("Creating New Player!");
        Player player = new Player();
        /*
        player.SetAggroList(new List<ICombatable>());
        player.SetLevel(1);
        player.SetCurrentHealth(100);
        player.SetMaxHealth(100);
        player.SetCurrentMana(100);
        player.SetMaxMana(100);
        player.SetCurrentExperience(0);
        player.SetMaxExperience(100);
        player.SetClassEnum(Classes.Priest);
        player.SetPlayerClass(Resources.Load<PlayerClass>($"Player Classes/{player.GetClassEnum()}"));
        player.SetLocation(new Vector3(2700, 12, 2200));
        player.SetScale(Vector3.one);
        player.SetRotation(Quaternion.identity);
        player.SetTargetStatus(TargetStatus.Alive);
        player.SetCombatStatus(CombatStatus.OutOfCombat);
        player.SetActionState(ActionState.Idle);
        player.SetVitality(5);
        player.SetWisdom(5);
        */
        player.name = name;

        List<Player> sameNamedPlayers = Database.instance.GetAllPlayers().Where(p => p.name == player.name).ToList();
        List<int> excludedIDs = sameNamedPlayers.Select(p => p.ID).ToList();
        do {
            player.ID = Random.Range(0, 9999999);
        } while (excludedIDs.Contains(player.ID));
        Database.instance.AddPlayer(player, credentials);
        Database.instance.AddClient(player, sender);
        Debug.Log("Added client to list");
        API.instance.CompleteLogin(sender, player.GetKey());
    }
}
