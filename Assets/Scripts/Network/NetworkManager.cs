using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string roomName = "VRRoom";
    [SerializeField] public Transform spawnLocation;
    [SerializeField] public Transform lobbyLocation;
    [SerializeField] public Transform sceneLocation;

    private readonly Dictionary<int, GameObject> _players = new();

    [SerializeField] private GameObject offlineUI;
    [SerializeField] private GameObject masterLobbyUI;
    [SerializeField] private GameObject runningSessionUI;


    void Start()
    {
        offlineUI.SetActive(false);
        masterLobbyUI.SetActive(false);
        runningSessionUI.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PUN] Connected to Master");
        offlineUI.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("[PUN] Joined room");

        // Setting nicknames for future easier recognititon
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.NickName = "Master";
        }
        else
        {
            PhotonNetwork.NickName = "Client";
        }

        Vector3 variation = (Vector3.right * Random.Range(-3f, 3f)) + (Vector3.forward * Random.Range(-3f, 3f)) + (Vector3.up * 2);

        var localPlayer = PhotonNetwork.Instantiate("PlayerVR", sceneLocation.position + variation, Quaternion.identity);
        
        _players[PhotonNetwork.LocalPlayer.ActorNumber] = localPlayer;

        offlineUI.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PUN] You are the master!");
             masterLobbyUI.SetActive(true);   // TODO. Delete this

        } else
        {
            Debug.Log("[PUN] You are the client!");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUN] Player {newPlayer.ActorNumber} connected");
        // Only MasterClient's logic
        if (PhotonNetwork.IsMasterClient && newPlayer.ActorNumber > 1)
        {
            masterLobbyUI.SetActive(true);
        //     TeleportAllPlayers(lobbyLocation);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUN] Player {otherPlayer.ActorNumber} left");
        // Remove and destroy their avatar if we have it registered
        if (_players.TryGetValue(otherPlayer.ActorNumber, out var go))
        {
            _players.Remove(otherPlayer.ActorNumber);
            Destroy(go);
        }
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("[PUN] Room created");
    }

    public override void OnCreateRoomFailed(short code, string msg) =>
      Debug.LogError($"[PUN] Host failed: {msg}");

    public override void OnJoinRoomFailed(short code, string msg) =>
      Debug.LogError($"[PUN] Join failed: {msg}");

    public void HostRoom()
    {
        Debug.Log("Hosting Room");
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRoom()
    {
        Debug.Log("Joining Room");
        PhotonNetwork.JoinRoom(roomName);
    }

    public string clientNickname()
    {
        return PhotonNetwork.NickName;
    }

    public void onSessionStarted()
    {
        masterLobbyUI.SetActive(false);
        runningSessionUI.SetActive(true);
    }

    public void onSessionFinished()
    {
        runningSessionUI.SetActive(false);
        masterLobbyUI.SetActive(true);
    }
}
