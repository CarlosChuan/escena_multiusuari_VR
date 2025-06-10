using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Objecte interactiu de xarxa.
/// </summary>  
public class NetworkInteractableObject : XRGrabInteractable
{
    // Component de xarxa.
    private PhotonView photonView;

    // Component de física.
    private Rigidbody rb;

    /// <summary>
    /// Quan es desa.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Obté el component de xarxa.
        photonView = GetComponent<PhotonView>();

        // Obté el component de física.
        rb = GetComponent<Rigidbody>();

        photonView.OwnershipTransfer = OwnershipOption.Request;
    }

    /// <summary>
    /// Quan s'habilita.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        photonView.OwnershipTransfer = OwnershipOption.Request;
        OwnershipManager.OwnershipTransferredEvent += OnGlobalOwnershipTransferred;
        EventManager.OnGrabMyObject += OnOwnerGrabOwnObject;
    }

    /// <summary>
    /// Quan s'ha deshabilitat.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        
        OwnershipManager.OwnershipTransferredEvent -= OnGlobalOwnershipTransferred;
        EventManager.OnGrabMyObject -= OnOwnerGrabOwnObject;
    }

    /// <summary>
    /// Quan s'ha agafat.
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Quan s'ha agafat, demana la propietat si no és el propietari.
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

        // Registra l'objecte agafat.
        PuzzleComponent puzzleComponent = GetComponent<PuzzleComponent>();
        var e = new ObjectPicked(puzzleComponent.groupName);
        EventLogger.Instance.Log(e);

        base.OnSelectEntered(args);

    }

    /// <summary>
    /// Quan s'ha deixat anar.
    /// </summary>
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

    /// <summary>
    /// Quan s'ha transferit la propietat.
    /// </summary>
    /// <param name="targetView">Vista de la xarxa.</param>
    /// <param name="previousOwner">Propietari anterior.</param>
    private void OnGlobalOwnershipTransferred(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonView)
        {
            Debug.Log("Ownership transferred. Now can manipulate Rigidbody.");
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Quan el propietari agafa el seu objecte.
    /// </summary>
    private void OnOwnerGrabOwnObject()
    {
        rb.isKinematic = true;
    }
}
