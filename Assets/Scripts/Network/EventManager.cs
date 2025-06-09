using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventManager : MonoBehaviour, IOnEventCallback
{

    [SerializeField] private TablePlacer tablePlacer;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private SceneController sceneController;

    public static event Action OnGrabMyObject;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case EventCodes.GrabbedMyObject:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");
                
                OnGrabMyObject?.Invoke();
                
                break;

            case EventCodes.CreateScene:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");
                
                object[] data = (object[])photonEvent.CustomData;
                int seed = (int)data[0];
                int sceneTypeIndex = (int)data[1];
                SceneType sceneType = (SceneType)sceneTypeIndex;
                
                Debug.Log("Generating scene");

                sceneController.GenerateRandomElements(sceneType, seed);

                break;

            case EventCodes.FinishScene:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");

                Debug.Log("Finishing scene");

                sceneController.FinishScene();

                break;
            default:
                break;
        }
    }
}
