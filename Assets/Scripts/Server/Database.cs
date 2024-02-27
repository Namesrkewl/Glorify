using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.ComponentModel;


public class Database : NetworkBehaviour {
    public static Database instance;
    private void Awake() {
        instance = this;
    }

    #region Players
    private readonly SyncDictionary<int, Player> players = new SyncDictionary<int, Player>();
    private readonly SyncDictionary<string, int> playerIDs = new SyncDictionary<string, int>(); 
    private readonly SyncDictionary<int, Key> playerKeys = new SyncDictionary<int, Key>();

    public Player GetPlayer(int ID) {
        players.TryGetValue(ID, out Player player);
        return player;
    }

    public void AddPlayer(Player player) {
        players.Add(player.GetID(), player);
    }

    public void RemovePlayer(int ID) {
        players.Remove(ID);
    }

    public Dictionary<int, Player> GetAllPlayers() {
        return players.Collection;
    }

    public int GetPlayerID(string name) {
        playerIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public void AddPlayerID(Player player) {
        playerIDs.Add(player.name, player.GetID());
    }

    public Dictionary<string, int> GetAllPlayerIDs() {
        return playerIDs.Collection;
    }

    public int GetPlayerCount() {
        return playerIDs.Count;
    }

    public Key GetPlayerKey(int ID) {
        playerKeys.TryGetValue(ID, out Key key);
        return key;
    }

    public void AddPlayerKey(int ID, Key key) {
        playerKeys.Add(ID, key);
    }

    public Dictionary<int, Key> GetAllPlayerKeys() {
        return playerKeys.Collection;
    }

    #endregion

    #region NPCs
    private readonly SyncDictionary<int, NPC> npcs = new SyncDictionary<int, NPC>();
    private readonly SyncDictionary<string, int> npcIDs = new SyncDictionary<string, int>();

    public NPC GetNPC(int ID) {
        npcs.TryGetValue(ID, out NPC npc);
        return npc;
    }

    public void AddNPC(NPC npc) {
        npcs.Add(npc.GetID(), npc);
    }

    public void RemoveNPC(int ID) {
        npcs.Remove(ID);
    }

    public int GetNPCID(string name) {
        npcIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public void AddNPCID(NPC npc) {
        npcIDs.Add(npc.name, npc.GetID());
    }

    public Dictionary<string, int> GetAllNPCIDs() {
        return npcIDs.Collection;
    }

    public int GetNPCCount() {
        return npcIDs.Count;
    }

    #endregion

    #region Spells
    private readonly SyncDictionary<int, Spell> spells = new SyncDictionary<int, Spell>();
    private readonly SyncDictionary<string, int> spellIDs = new SyncDictionary<string, int>();

    public Spell GetSpell(int ID) {
        spells.TryGetValue(ID, out Spell spell);
        return spell;
    }

    public void AddSpell(Spell spell) {
        spells.Add(spell.GetID(), spell);
    }

    public void RemoveSpell(int ID) {
        spells.Remove(ID);
    }

    public int GetSpellID(string name) {
        spellIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public void AddSpellID(Spell spell) {
        spellIDs.Add(spell.name, spell.GetID());
    }

    public Dictionary<string, int> GetAllSpellIDs() {
        return spellIDs.Collection;
    }

    public int GetSpellCount() {
        return spellIDs.Count;
    }

    #endregion

    #region Items
    private readonly SyncDictionary<int, Item> items = new SyncDictionary<int, Item>();
    private readonly SyncDictionary<string, int> itemIDs = new SyncDictionary<string, int>();

    public Item GetItem(int ID) {
        items.TryGetValue(ID, out Item item);
        return item;
    }

    public void AddItem(Item item) {
        items.Add(item.GetID(), item);
    }

    public void RemoveItem(int ID) {
        items.Remove(ID);
    }

    public int GetItemID(string name) {
        itemIDs.TryGetValue(name, out int ID);
        return ID;
    }

    public void AddItemID(Item item) {
        itemIDs.Add(item.name, item.GetID());
    }

    public Dictionary<string, int> GetAllItemIDs() {
        return itemIDs.Collection;
    }
    public int GetItemCount() {
        return itemIDs.Count;
    }

    #endregion
}
