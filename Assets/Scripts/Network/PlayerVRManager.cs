using Photon.Pun;
using UnityEngine;
using System;

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

    void Start()
    {
        if (photonView.IsMine)
        {
            DisableLocalVisuals(transform);
        }

        if (photonView.IsMine && localHeadTransform == null)
        {
            var cam = Camera.main;
            if (cam != null) { 
                Debug.Log("Setting camera as headTransform");
                localHeadTransform = cam.transform;
            } else
            {
                Debug.Log("FollowHMD: no Camera.main found!");
            }

            var leftController = GameObject.FindGameObjectWithTag("LeftController");
            if (leftController != null)
            {
                Debug.Log("Setting left controller as LHand");
                localLHandTransform = leftController.transform;
            } else
            {
                Debug.Log("No left controller found");
            }

            var rightController = GameObject.FindGameObjectWithTag("RightController");
            if (rightController != null)
            {
                Debug.Log("Setting right controller as RHand");
                localRHandTransform = rightController.transform;
            }
            else
            {
                Debug.Log("No right controller found");
            }
        }
    }
    private void DisableLocalVisuals(Transform parent)
    {
        // Disable on this node, then recurse into children
        var mr = parent.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        var mf = parent.GetComponent<MeshFilter>();
        if (mf != null) mf.mesh = null;

        var bc = parent.GetComponent<BoxCollider>();
        if (bc != null) bc.enabled = false;

        // Recurse
        foreach (Transform child in parent)
        {
            DisableLocalVisuals(child);
        }
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        // remote: lerp from current to the networked head?pos
        if (localHeadTransform)
        {
            virtualHeadTransform.position = localHeadTransform.position;
            virtualHeadTransform.rotation = localHeadTransform.rotation;
        } else if (retryCountHead > 0)
        {
            Debug.Log($"Head not existing. Trying to find it. {retryCountHead} chances left.");

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

        if (localLHandTransform)
        {
            virtualLHandTransform.position = localLHandTransform.position;
            virtualLHandTransform.rotation = localLHandTransform.rotation;
        } else if (retryCountL > 0)
        {
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
            
        if (localRHandTransform)
        {
            virtualRHandTransform.position = localRHandTransform.position;
            virtualRHandTransform.rotation = localRHandTransform.rotation;
        } else if(retryCountR > 0)
        {
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
