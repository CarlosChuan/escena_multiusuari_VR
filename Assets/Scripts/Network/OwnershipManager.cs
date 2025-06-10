using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

/// <summary>
/// Gestor de propietat.
/// </summary>
public class OwnershipManager : MonoBehaviourPun, IPunOwnershipCallbacks
{
    /// <summary>
    /// Event de transferència de propietat.
    /// </summary>
    public static event Action<PhotonView, Player> OwnershipTransferredEvent;

    /// <summary>
    /// Quan està habilitat.
    /// </summary>
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// Quan està desabilitat.
    /// </summary>
    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Quan es demana la propietat.
    /// </summary>
    /// <param name="targetView">Vista del target.</param>
    /// <param name="requestingPlayer">Jugador que demana la propietat.</param>
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log($"Ownership requested by: {requestingPlayer.NickName}");
        targetView.TransferOwnership(requestingPlayer);
    }

    /// <summary>
    /// Quan es transfereix la propietat.
    /// </summary>
    /// <param name="targetView">Vista del target.</param>
    /// <param name="previousOwner">Propietari anterior.</param>
    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log($"Ownership transferred from {previousOwner.NickName} to {targetView.Owner.NickName}");
        OwnershipTransferredEvent?.Invoke(targetView, previousOwner);
    }

    /// <summary>
    /// Quan falla la transferència de propietat.
    /// </summary>
    /// <param name="targetView">Vista del target.</param>
    /// <param name="senderOfFailedRequest">Remitent de la petició.</param>
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        Debug.LogError($"Ownership transfer FAILED from {senderOfFailedRequest.NickName} for view {targetView.ViewID}");
    }
}