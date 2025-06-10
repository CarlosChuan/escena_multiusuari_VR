using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Plataforma on deixar objectes.
/// </summary>
public class SnapSlot : MonoBehaviour
{
    // Component de la caixa de col·lisió.
    [HideInInspector] public SnapBox snapBox;

    // Índex del slot.
    [HideInInspector] public int slotIndex;

    // Component de la escena.
    [SerializeField] private SceneController controller;

    // Material de la base.
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material incorrectMaterial;

    // Component del renderer.
    private MeshRenderer meshRenderer;

    /// <summary>
    /// Quan comence.
    /// </summary>
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

    /// <summary>
    /// Quan un objecte entra en el slot.
    /// </summary>
    /// <param name="other">Objecte que entra.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snappable") && other.TryGetComponent<PuzzleComponent>(out var _))
        {
            // Intenta col·locar l'objecte en el slot.
            bool correct = snapBox.TrySnap(other.gameObject, slotIndex);

            // Si l'objecte està col·locat correctament.
            if (correct)
            {
                // Obté el component de la seqüència.
                PuzzleElementData puzzleElement = controller.ElementsSequence[slotIndex];

                // Obté el slot.
                Slot slot = snapBox.slots[slotIndex];

                // Component de xarxa.
                PhotonView photonView = other.GetComponent<PhotonView>();

                // Si el slot està ocupat i el nom del grup coincideix.
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
                    // Si el slot està ocupat i el nom del grup no coincideix.
                    if (photonView && photonView.IsMine)
                    {
                        var e = new ErrorPlacement(slotIndex);
                        EventLogger.Instance.Log(e);
                    }
                    meshRenderer.material = incorrectMaterial;
                }
            } else
            {
                // Si l'objecte no està col·locat correctament.
                meshRenderer.material = incorrectMaterial;
            }
        }
    }

    /// <summary>
    /// Quan un objecte surt del slot.
    /// </summary>
    /// <param name="other">Objecte que surt.</param>
    private void OnTriggerExit(Collider other)
    {
        // Si l'objecte és un objecte col·locable i té un component PuzzleComponent.
        if (other.CompareTag("Snappable") && other.TryGetComponent<PuzzleComponent>(out var _))
        {
            // Intenta desacoplar l'objecte del slot.
            snapBox.TryDetach(other.gameObject, slotIndex);

            // Si el slot no està ocupat.
            if (!snapBox.slots[slotIndex].IsOccupied())
            {
                // Restaura el material de la base.
                meshRenderer.material = baseMaterial;
            }
        }
    }
}
