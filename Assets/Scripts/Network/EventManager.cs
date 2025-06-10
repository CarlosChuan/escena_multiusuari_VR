using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Gestor d'esdeveniments de xarxa.
/// </summary>
public class EventManager : MonoBehaviour, IOnEventCallback
{
    // Component de la taula.
    [SerializeField] private TablePlacer tablePlacer;

    // Component de xarxa.
    [SerializeField] private NetworkManager networkManager;

    // Component de la escena.
    [SerializeField] private SceneController sceneController;

    // Event de agafament d'objecte.
    public static event Action OnGrabMyObject;

    /// <summary>
    /// Quan s'habilita.
    /// </summary>
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// Quan s'ha deshabilitat.
    /// </summary>
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Quan s'ha rebut un esdeveniment.
    /// </summary>
    /// <param name="photonEvent">Event de xarxa.</param>
    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            // Event de agafament d'objecte.
            case EventCodes.GrabbedMyObject:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");

                // Invoca l'esdeveniment de xarxa de agafament d'objecte.
                OnGrabMyObject?.Invoke();

                break;

            // Event de creació d'escena.
            case EventCodes.CreateScene:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");

                // Obté el seed i el tipus d'escena.
                object[] data = (object[])photonEvent.CustomData;
                int seed = (int)data[0];
                int sceneTypeIndex = (int)data[1];
                SceneType sceneType = (SceneType)sceneTypeIndex;

                // Genera elements aleatoris.
                Debug.Log("Generating scene");

                sceneController.GenerateRandomElements(sceneType, seed);

                break;

            // Event de finalització d'escena.
            case EventCodes.FinishScene:
                Debug.Log($"Event recieved! {photonEvent.Code}, parameters: {JsonUtility.ToJson(photonEvent.Parameters, prettyPrint: true)}, custom data: {JsonUtility.ToJson(photonEvent.CustomData, prettyPrint: true)}");

                Debug.Log("Finishing scene");

                // Finalitza l'escena.
                sceneController.FinishScene();

                break;
            default:
                break;
        }
    }
}
