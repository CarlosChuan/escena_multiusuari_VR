using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkInteractableObject : XRGrabInteractable
{
    private PhotonView photonView;
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        // Ensure ownership transfer is set to Request
        photonView.OwnershipTransfer = OwnershipOption.Request;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        OwnershipManager.OwnershipTransferredEvent += OnGlobalOwnershipTransferred;
        EventManager.OnGrabMyObject += OnOwnerGrabOwnObject;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OwnershipManager.OwnershipTransferredEvent -= OnGlobalOwnershipTransferred;
        EventManager.OnGrabMyObject -= OnOwnerGrabOwnObject;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // When grabbed, request ownership if not owner
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log("Ownership requested via grab.");
        } else
        {
            object[] data = new object[] {};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(EventCodes.GrabbedMyObject, data, raiseEventOptions, sendOptions);
        }

        // Logging object grabbed
        PuzzleComponent puzzleComponent = GetComponent<PuzzleComponent>();
        var e = new ObjectPicked(puzzleComponent.groupName);
        EventLogger.Instance.Log(e);

        base.OnSelectEntered(args);

    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (photonView.IsMine)
        {
            rb.isKinematic = false;
        } else
        {
            rb.isKinematic = true;
        }

        base.OnSelectExited(args);
    }

    private void OnGlobalOwnershipTransferred(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonView)
        {
            Debug.Log("Ownership transferred. Now can manipulate Rigidbody.");
            rb.isKinematic = true;
        }
    }

    private void OnOwnerGrabOwnObject()
    {
        rb.isKinematic = true;
    }
}
