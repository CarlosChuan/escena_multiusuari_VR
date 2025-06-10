using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Gestiona la sessió global, tant xarxa com la escena.
/// </summary>
public class SessionManager : MonoBehaviour
{
    // Components de la sessió.
    [SerializeField] SceneController sceneController;
    [SerializeField] NetworkManager networkController;

    // Tipus de distribució seleccionat.
    public SceneType selectedSceneType;

    // Inicialització.
    void Start()
    {
        if (networkController == null)
            networkController = FindObjectOfType<NetworkManager>();

        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();
    }

    /// <summary>
    /// Creació d'una sala.
    /// </summary>
    public void HostRoom()
    {
        networkController.HostRoom();
    }

    /// <summary>
    /// Uni un jugador a una sala.
    /// </summary>
    public void JoinRoom()
    {
        networkController.JoinRoom();
    }

    /// <summary>
    /// Crea una escena.
    /// </summary>
    /// <param name="sceneTypeIndex">Índex del tipus de distribució.</param>
    public void CreateScene(int sceneTypeIndex)
    {
        Debug.Log($"Creating Scene {sceneTypeIndex} = {(SceneType)sceneTypeIndex}");

        int seed = Random.Range(0, 1000);

        object[] data = new object[]
        {
            seed, sceneTypeIndex
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        // Invoca l'esdeveniment de xarxa de creació d'escena.
        PhotonNetwork.RaiseEvent(EventCodes.CreateScene, data, raiseEventOptions, sendOptions);

        // Inicia la sessió.
        networkController.onSessionStarted();
    }

    /// <summary>
    /// Finalitza la escena.
    /// </summary>
    public void FinishScene()
    {
        Debug.Log($"Finishing current scene and going back to lobby");

        object[] data = new object[] {};

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        // Invoca l'esdeveniment de xarxa de finalització d'escena.
        PhotonNetwork.RaiseEvent(EventCodes.FinishScene, data, raiseEventOptions, sendOptions);

        // Inicia la sessió.
        networkController.onSessionFinished();
    }
}
