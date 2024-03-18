using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.ComponentModel;
using System.Linq;
using FishNet.Demo.AdditiveScenes;
using FishNet.CodeGenerating;
using System;


public class Database : MonoBehaviour {
    public static Database instance;
    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            UpdateDictionaries();
        }
    }

    private void UpdateDictionaries() {
        playerSearch.Clear();
        foreach (Player player in players) {
            playerSearch.Add(player.GetKey(), player);
        }
        clientSearch.Clear();
    }

    #region Players
    [AllowMutableSyncType]
    private List<Player> players = new List<Player>();
    private readonly List<Credentials> credentials = new List<Credentials>();
    private readonly Dictionary<Key, Player> playerSearch = new Dictionary<Key, Player>();
    private readonly Dictionary<Player, NetworkConnection> clientSearch = new Dictionary<Player, NetworkConnection>();
    private readonly Dictionary<Credentials, Player> users = new Dictionary<Credentials, Player>();

    public Player GetPlayer(Key key) {
        playerSearch.TryGetValue(key, out Player player);
        return player;
    }

    public bool CheckAvailability(Credentials input) {
        if (credentials.Any(c => c.username == input.username)) {
            Debug.Log("Duplicate found!");
            return false;
        } else {
            Debug.Log("Name available!");
            return true;
        }
    }

    public Player GetUser(Credentials credentials) {
        users.TryGetValue(credentials, out Player player);
        return player;
    }

    public void AddPlayer(Player player, Credentials _credentials) {
        Debug.Log(player.name);
        Debug.Log(player.GetKey().name);
        players.Add(player);
        playerSearch.Add(player.GetKey(), player);
        credentials.Add(_credentials);
        users.Add(_credentials, player);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public List<Player> GetAllPlayers() {
        return players;
    }

    public NetworkConnection GetClient(Player player) {
        clientSearch.TryGetValue(player, out NetworkConnection client);
        return client;
    }

    public void AddClient(Player player, NetworkConnection client) {
        clientSearch.Add(player, client);
    }

    public void RemoveClient(Player player) {
        clientSearch.Remove(player);
    }

    #endregion

    public Character GetTarget(ITargetable target) {
        if (target as PlayerBehaviour) {
            Key key = target.GetKey();
            return GetPlayer(key);
        } else if (target as NPCBehaviour) {
            return (target as NPCBehaviour).npc.Value;
        } else {
            return null;
        }
    }
}