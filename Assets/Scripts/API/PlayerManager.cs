using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing.Logging;
using System.Linq;
using UnityEditor;
public class PlayerManager : NetworkBehaviour
{

    public static PlayerManager instance;
    public NetworkPlayerController localPlayer;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [Server(Logging = LoggingType.Off)]
    public void CreatePlayer(string name, Credentials credentials, NetworkConnection sender)
    {
        Debug.Log("Creating New Player!");
        Player player = new Player();
        player.InitializePlayer();
        player.name = name;

        List<Player> sameNamedPlayers = Database.instance.GetAllPlayers().Where(p => p.name == player.name).ToList();
        List<int> excludedIDs = sameNamedPlayers.Select(p => p.ID).ToList();
        do
        {
            player.ID = Random.Range(0, 9999999);
        } while (excludedIDs.Contains(player.ID));
        Database.instance.AddPlayer(player, credentials);
        Database.instance.AddClient(player, sender);
        Debug.Log("Added client to list");
        API.instance.CompleteLogin(sender, player.GetKey());
    }

    //Added by Dudesss
    public void SetPlayer(NetworkPlayerController _localPlayer)
    {
        localPlayer = _localPlayer;
    }
}
