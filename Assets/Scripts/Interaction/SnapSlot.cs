using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SnapSlot : MonoBehaviour
{
    [HideInInspector] public SnapBox snapBox;
    [HideInInspector] public int slotIndex;
    [SerializeField] private SceneController controller;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material incorrectMaterial;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        if (meshRenderer == null)
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
            this.meshRenderer.material = baseMaterial;
        }

        if (controller == null)
        {
            controller = FindObjectOfType<SceneController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snappable") && other.TryGetComponent<PuzzleComponent>(out var _))
        {
            bool correct = snapBox.TrySnap(other.gameObject, slotIndex);
            if (correct)
            {
                PuzzleElementData puzzleElement = controller.ElementsSequence[slotIndex];
                Slot slot = snapBox.slots[slotIndex];

                PhotonView photonView = other.GetComponent<PhotonView>();
                if (slot != null && slot.occupiedPuzzleElementData.groupName == puzzleElement.groupName)
                {
                    if (photonView && photonView.IsMine)
                    {
                        var e = new ObjectPlaced(puzzleElement.groupName, slotIndex);
                        EventLogger.Instance.Log(e);
                    }
                    meshRenderer.material = correctMaterial;
                } else
                {
                    if (photonView && photonView.IsMine)
                    {
                        var e = new ErrorPlacement(slotIndex);
                        EventLogger.Instance.Log(e);
                    }
                    meshRenderer.material = incorrectMaterial;
                }
            } else
            {
                meshRenderer.material = incorrectMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Snappable") && other.TryGetComponent<PuzzleComponent>(out var _))
        {
            snapBox.TryDetach(other.gameObject, slotIndex);
            if (!snapBox.slots[slotIndex].IsOccupied())
            {
                meshRenderer.material = baseMaterial;
            }
        }
    }
}
