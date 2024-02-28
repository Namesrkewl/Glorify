using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.ComponentModel;
using System.Linq;
using FishNet.Demo.AdditiveScenes;


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
        npcSearch.Clear();
        foreach (Player player in players) {
            playerSearch.Add(player.key, player);
        }
        foreach (NPC npc in npcs) {
            npcSearch.Add(npc.key, npc);
        }
    }

    #region Players
    public readonly SyncList<Player> players = new SyncList<Player>();
    public readonly List<Credentials> credentials = new List<Credentials>();
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

    #endregion

    #region NPCs
    public readonly List<NPC> npcs = new List<NPC>();
    private readonly Dictionary<Key, NPC> npcSearch = new Dictionary<Key, NPC>();

    public NPC GetNPC(Key key) {
        npcSearch.TryGetValue(key, out NPC npc);
        return npc;
    }

    #endregion

    #region Spells
    private readonly Dictionary<int, Spell> spells = new Dictionary<int, Spell>();
    private readonly Dictionary<string, int> spellIDs = new Dictionary<string, int>();

    public Spell GetSpell(int ID) {
        spells.TryGetValue(ID, out Spell spell);
        return spell;
    }

    public int GetSpellID(string name) {
        spellIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public void AddSpellID(Spell spell) {
        //spellIDs.Add(spell.name, spell.GetID());
    }

    public Dictionary<string, int> GetAllSpellIDs() {
        return spellIDs;
    }

    public int GetSpellCount() {
        return spellIDs.Count;
    }

    #endregion

    #region Items
    private readonly Dictionary<int, Item> items = new Dictionary<int, Item>();
    private readonly Dictionary<string, int> itemIDs = new Dictionary<string, int>();

    public Item GetItem(int ID) {
        items.TryGetValue(ID, out Item item);
        return item;
    }

    public void AddItem(Item item) {
        //items.Add(item.GetKey(), item);
    }

    public void RemoveItem(int ID) {
        items.Remove(ID);
    }

    public int GetItemID(string name) {
        itemIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public Dictionary<string, int> GetAllItemIDs() {
        return itemIDs;
    }
    public int GetItemCount() {
        return itemIDs.Count;
    }

    #endregion

    public Character GetTarget(ITargetable target) {
        Character character = null;
        Key key = target.GetKey();
        if (target as PlayerBehaviour) {
            character = GetPlayer(key);
        } else if (target as NPCBehaviour) {
            character = GetNPC(key);
        }
        return character;
    }
}
