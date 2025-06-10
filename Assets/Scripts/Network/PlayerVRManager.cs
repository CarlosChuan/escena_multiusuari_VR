using Photon.Pun;
using UnityEngine;
using System;

/// <summary>
/// Gestor de la realitat virtual.
/// </summary>
public class PlayerVRManager : MonoBehaviourPun
{
    [Header("Local rig components")]
    private Transform localHeadTransform;
    private Transform localLHandTransform;
    private Transform localRHandTransform;

    [Header("Virtual rig components")]
    [SerializeField] private Transform virtualHeadTransform;
    [SerializeField] private Transform virtualLHandTransform;
    [SerializeField] private Transform virtualRHandTransform;

    private int retryCountHead, retryCountR, retryCountL = 5;

    /// <summary>
    /// Quan es fa start.
    /// </summary>
    void Start()
    {
        // Si aquest component està associat al jugador local.
        if (photonView.IsMine)
        {
            // Desabilita els visuals locals.
            DisableLocalVisuals(transform);
        }

        // Si el jugador és el mestre i no està vinculat el cap.
        if (photonView.IsMine && localHeadTransform == null)
        {
            // Obté la càmera principal.
            var cam = Camera.main;
            if (cam != null) { 
                Debug.Log("Setting camera as headTransform");
                // I es vincula al cap.
                localHeadTransform = cam.transform;
            } else
            {
                Debug.Log("FollowHMD: no Camera.main found!");
            }

            // Obté el control esquerre.
            var leftController = GameObject.FindGameObjectWithTag("LeftController");
            if (leftController != null)
            {
                Debug.Log("Setting left controller as LHand");
                // I es vincula al control esquerre.
                localLHandTransform = leftController.transform;
            } else
            {
                Debug.Log("No left controller found");
            }

            // Obté el control dret.
            var rightController = GameObject.FindGameObjectWithTag("RightController");
            if (rightController != null)
            {
                Debug.Log("Setting right controller as RHand");
                // I es vincula al control dret.
                localRHandTransform = rightController.transform;
            }
            else
            {
                Debug.Log("No right controller found");
            }
        }
    }

    /// <summary>
    /// Desabilita els visuals locals de forma recursiva.
    /// </summary>
    /// <param name="parent">Parent del node.</param>   
    private void DisableLocalVisuals(Transform parent)
    {
        // Desabilita el MeshRenderer en aquest node, llavors recorre els fills.
        var mr = parent.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        var mf = parent.GetComponent<MeshFilter>();
        if (mf != null) mf.mesh = null;

        var bc = parent.GetComponent<BoxCollider>();
        if (bc != null) bc.enabled = false;

        // Recorre tots els fills.
        foreach (Transform child in parent)
        {
            DisableLocalVisuals(child);
        }
    }

    /// <summary>
    /// Quan es fa update.
    /// </summary>
    void Update()
    {
        // Si el jugador no és el mestre i està connectat, no es fa res.
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        // remote: lerp from current to the networked head?pos
        // Si tenim el cap local.
        if (localHeadTransform)
        {
            // Actualitza la posició i rotació del cap virtual.
            virtualHeadTransform.position = localHeadTransform.position;
            virtualHeadTransform.rotation = localHeadTransform.rotation;
        } else if (retryCountHead > 0) // Si no tenim el cap local i hi ha oportunitat de tornar a trobar-ho.
        {
            Debug.Log($"Head not existing. Trying to find it. {retryCountHead} chances left.");

            // Obté la càmera principal.
            var cam = Camera.main;
            if (cam != null)
            {
                Debug.Log("Setting camera as headTransform");
                localHeadTransform = cam.transform;
            }
            else
            {
                Debug.Log("No camera found. Reducing 1 the chances left.");
                retryCountHead--;
            }

            if (retryCountHead == 0)
            {
                Debug.Log("No more chances left. GL");
            }
        }

        // Si tenim el control esquerre local.
        if (localLHandTransform)
        {
            // Actualitza la posició i rotació del control esquerre virtual.
            virtualLHandTransform.position = localLHandTransform.position;
            virtualLHandTransform.rotation = localLHandTransform.rotation;
        } else if (retryCountL > 0) // Si no tenim el control esquerre local i hi ha oportunitat de tornar a trobar-ho.
        {
            // Obté el control esquerre.
            var leftController = GameObject.FindGameObjectWithTag("LeftController");
            if (leftController != null)
            {
                Debug.Log("Setting left controller as LHand");
                localLHandTransform = leftController.transform;
            }
            else
            {
                Debug.Log("No left controller found. Reducing 1 the chances left.");
                retryCountL--;
            }

            if (retryCountL == 0)
            {
                Debug.Log("No more chances left. GL");
            }
        }

        // Si tenim el control dret local.
        if (localRHandTransform)
        {
            // Actualitza la posició i rotació del control dret virtual.
            virtualRHandTransform.position = localRHandTransform.position;
            virtualRHandTransform.rotation = localRHandTransform.rotation;
        } else if(retryCountR > 0) // Si no tenim el control dret local i hi ha oportunitat de tornar a trobar-ho.
        {
            // Obté el control dret.
            var rightController = GameObject.FindGameObjectWithTag("RightController");
            if (rightController != null)
            {
                Debug.Log("Setting right controller as RHand");
                localRHandTransform = rightController.transform;
            }
            else
            {
                Debug.Log("No right controller found. Reducing 1 the chances left.");
                retryCountR--;
            }

            if (retryCountR == 0)
            {
                Debug.Log("No more chances left. GL");
            }
        }
    }
}
