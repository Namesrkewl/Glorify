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
            players.OnChange += players_OnChange;
        }
    }

    private void UpdateDictionaries() {
        playerSearch.Clear();
        foreach (Player player in players) {
            playerSearch.Add(player.key, player);
        }
        clientSearch.Clear();
    }

    #region Players
    [AllowMutableSyncType]
    private SyncList<Player> players = new SyncList<Player>();
    private readonly List<Credentials> credentials = new List<Credentials>();
    private readonly Dictionary<Key, Player> playerSearch = new Dictionary<Key, Player>();
    private readonly Dictionary<Key, NetworkConnection> clientSearch = new Dictionary<Key, NetworkConnection>();
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

    public void UpdatePlayer(Player player) {
        players.Dirty(player);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public SyncList<Player> GetAllPlayers() {
        return players;
    }

    public NetworkConnection GetClient(Key key) {
        clientSearch.TryGetValue(key, out NetworkConnection client);
        return client;
    }

    public void AddClient(Key key, NetworkConnection client) {
        clientSearch.Add(key, client);
    }

    public void RemoveClient(Key key) {
        clientSearch.Remove(key);
    }

    private void players_OnChange(SyncListOperation op, int index, Player oldPlayer, Player newPlayer, bool asServer) {
        switch (op) {
            /* An object was added to the list. Index
            * will be where it was added, which will be the end
            * of the list, while newItem is the value added. */
            case SyncListOperation.Add:
                break;
            /* An object was removed from the list. Index
            * is from where the object was removed. oldItem
            * will contain the removed item. */
            case SyncListOperation.RemoveAt:
                break;
            /* An object was inserted into the list. Index
            * is where the obejct was inserted. newItem
            * contains the item inserted. */
            case SyncListOperation.Insert:
                break;
            /* An object replaced another. Index
            * is where the object was replaced. oldItem
            * is the item that was replaced, while
            * newItem is the item which now has it's place. */
            case SyncListOperation.Set:
                UIManager.instance.UpdatePlayerInformation(GetClient(newPlayer.key), newPlayer);
                break;
            /* All objects have been cleared. Index, oldValue,
            * and newValue are default. */
            case SyncListOperation.Clear:
                break;
            /* When complete calls all changes have been
            * made to the collection. You may use this
            * to refresh information in relation to
            * the list changes, rather than doing so
            * after every entry change. Like Clear
            * Index, oldItem, and newItem are all default. */
            case SyncListOperation.Complete:
                break;
        }
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