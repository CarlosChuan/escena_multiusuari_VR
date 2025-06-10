using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Gestor de xarxa.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Nom de la sala.
    [SerializeField] string roomName = "VRRoom";

    // Lloc de spawn.
    [SerializeField] public Transform spawnLocation;

    // Lloc de la sala.
    [SerializeField] public Transform lobbyLocation;

    // Lloc de la sala.
    [SerializeField] public Transform sceneLocation;

    // Diccionari de jugadors.
    private readonly Dictionary<int, GameObject> _players = new();

    // UI de la sala offline.
    [SerializeField] private GameObject offlineUI;
    [SerializeField] private GameObject masterLobbyUI;
    [SerializeField] private GameObject runningSessionUI;

    /// <summary>
    /// Quan comença.
    /// </summary>
    void Start()
    {
        offlineUI.SetActive(false);
        masterLobbyUI.SetActive(false);
        runningSessionUI.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Quan està connectat a la xarxa.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("[PUN] Connected to Master");
        offlineUI.SetActive(true);
    }

    /// <summary>
    /// Quan s'ha unit a la sala.
    /// </summary>  
    public override void OnJoinedRoom()
    {
        Debug.Log("[PUN] Joined room");

        // Defineix el nom del jugador.
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.NickName = "Master";
        }
        else
        {
            PhotonNetwork.NickName = "Client";
        }

        // Defineix la variació de la posició.
        Vector3 variation = (Vector3.right * Random.Range(-3f, 3f)) + (Vector3.forward * Random.Range(-3f, 3f)) + (Vector3.up * 2);

        // Instancia el jugador.
        var localPlayer = PhotonNetwork.Instantiate("PlayerVR", sceneLocation.position + variation, Quaternion.identity);

        // Afegeix el jugador al diccionari.
        _players[PhotonNetwork.LocalPlayer.ActorNumber] = localPlayer;

        offlineUI.SetActive(false);

        // Si el jugador és el mestre.
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[PUN] You are the master!");

            // Mostra la UI de la sala mestre.
            masterLobbyUI.SetActive(true);

        } else
        {
            Debug.Log("[PUN] You are the client!");
        }
    }

    /// <summary>
    /// Quan un jugador s'ha unit a la sala.
    /// </summary>
    /// <param name="newPlayer">Jugador nou.</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[PUN] Player {newPlayer.ActorNumber} connected");

        // Si el jugador és el mestre i el nombre d'actors és major que 1.
        if (PhotonNetwork.IsMasterClient && newPlayer.ActorNumber > 1)
        {
            // Mostra la UI de la sala mestre.
            masterLobbyUI.SetActive(true);
        }
    }

    /// <summary>
    /// Quan un jugador ha marxat de la sala.
    /// </summary>
    /// <param name="otherPlayer">Jugador que ha marxat.</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[PUN] Player {otherPlayer.ActorNumber} left");

        // Si el jugador està registrat.
        if (_players.TryGetValue(otherPlayer.ActorNumber, out var go))
        {
            _players.Remove(otherPlayer.ActorNumber);
            Destroy(go);
        }
    }

    /// <summary>
    /// Quan s'ha creat la sala.
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("[PUN] Room created");
    }

    /// <summary>
    /// Quan falla la creació de la sala.
    /// </summary>
    /// <param name="code">Codi d'error.</param>
    /// <param name="msg">Missatge d'error.</param>
    public override void OnCreateRoomFailed(short code, string msg) =>
      Debug.LogError($"[PUN] Host failed! {code}: {msg}");

    /// <summary>
    /// Quan falla la unió a la sala.
    /// </summary>
    /// <param name="code">Codi d'error.</param>
    /// <param name="msg">Missatge d'error.</param>
    public override void OnJoinRoomFailed(short code, string msg) =>
      Debug.LogError($"[PUN] Join failed! {code}: {msg}");

    /// <summary>
    /// Quan s'ha creat la sala i n'es el propietari.
    /// </summary>
    public void HostRoom()
    {
        Debug.Log("Hosting Room");
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
    }

    /// <summary>
    /// Quan s'ha unit a la sala.
    /// </summary>
    public void JoinRoom()
    {
        Debug.Log("Joining Room");
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// Obté el nom del jugador.
    /// </summary>
    /// <returns>Nom del jugador.</returns>
    public string clientNickname()
    {
        return PhotonNetwork.NickName;
    }

    /// <summary>
    /// Quan s'ha iniciat la sessió.
    /// </summary>
    public void onSessionStarted()
    {
        masterLobbyUI.SetActive(false);
        runningSessionUI.SetActive(true);
    }

    /// <summary>
    /// Quan s'ha finalitzat la sessió.
    /// </summary>
    public void onSessionFinished()
    {
        runningSessionUI.SetActive(false);
        masterLobbyUI.SetActive(true);
    }

    /// <summary>
    /// Quan s'ha tancat l'aplicació.
    /// </summary>
    private void OnApplicationQuit()
    {
        PhotonNetwork.LeaveRoom();
    }
}
