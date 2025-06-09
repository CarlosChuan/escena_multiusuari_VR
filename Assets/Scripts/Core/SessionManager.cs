using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class SessionManager : MonoBehaviour
{
    [SerializeField] SceneController sceneController;
    [SerializeField] NetworkManager networkController;

    public SceneType selectedSceneType;


    void Start()
    {
        if (networkController == null)
            networkController = FindObjectOfType<NetworkManager>();

        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();
    }

    // Network Management
    public void HostRoom()
    {
        networkController.HostRoom();
    }

    public void JoinRoom()
    {
        networkController.JoinRoom();
    }

    // Session Management
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

        PhotonNetwork.RaiseEvent(EventCodes.CreateScene, data, raiseEventOptions, sendOptions);

        networkController.onSessionStarted();
    }

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

        PhotonNetwork.RaiseEvent(EventCodes.FinishScene, data, raiseEventOptions, sendOptions);

        networkController.onSessionFinished();
    }
}
