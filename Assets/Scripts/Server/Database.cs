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


public class Database : NetworkBehaviour {
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
            playerSearch.Add(player.key, player);
        }
    }

    #region Players
    [AllowMutableSyncType] private SyncList<Player> players = new SyncList<Player>();
    private readonly List<Credentials> credentials = new List<Credentials>();
    private readonly Dictionary<Key, Player> playerSearch = new Dictionary<Key, Player>();
    private readonly Dictionary<Credentials, Key> users = new Dictionary<Credentials, Key>();

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

    public Key GetUser(Credentials credentials) {
        users.TryGetValue(credentials, out Key key);
        return key;
    }

    public void AddPlayer(Player player, Credentials _credentials) {
        players.Add(player);
        playerSearch.Add(player.key, player);
        credentials.Add(_credentials);
        users.Add(_credentials, player.key);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public List<Player> GetAllPlayers() {
        return players.Collection;
    }

    #endregion

    public Character GetTarget(ITargetable target) {
        if (target as PlayerBehaviour) {
            Key key = target.GetKey();
            return GetPlayer(key);
        } else if (target as NPCBehaviour) {
            return (target as NPCBehaviour).npc;
        } else {
            return null;
        }
    }

}