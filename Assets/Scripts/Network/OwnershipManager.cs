using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class OwnershipManager : MonoBehaviourPun, IPunOwnershipCallbacks
{
    public static event Action<PhotonView, Player> OwnershipTransferredEvent;

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log($"Ownership requested by: {requestingPlayer.NickName}");
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log($"Ownership transferred from {previousOwner.NickName} to {targetView.Owner.NickName}");
        OwnershipTransferredEvent?.Invoke(targetView, previousOwner);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        Debug.LogError($"Ownership transfer FAILED from {senderOfFailedRequest.NickName} for view {targetView.ViewID}");
    }
}