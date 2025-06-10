using TMPro;
using UnityEngine;

/// <summary>
/// Mostra el nombre de peces col·locades.
/// </summary>
public class SnapCounterDisplay : MonoBehaviour
{
    /// <summary>
    /// Caixa de col·locació.
    /// </summary>
    public SnapBox snapBox;

    /// <summary>
    /// Controlador de la escena.
    /// </summary>
    public SceneController sceneController;

    /// <summary>
    /// Text de l'element UI.
    /// </summary>
    public TMP_Text displayText;

    /// <summary>
    /// Quan està habilitat.
    /// </summary>
    void OnEnable()
    {
        if (snapBox!= null)
        {
            snapBox.OnSnapChange += UpdateText;
        }
    }

    /// <summary>
    /// Quan està desabilitat.
    /// </summary>
    void OnDisable()
    {
        if (snapBox != null)
        {
            snapBox.OnSnapChange -= UpdateText;
        }
    }

    /// <summary>
    /// Actualitza el text.
    /// </summary>
    private void UpdateText()
    {
        if (displayText != null)
        {
            int wellPlaced = 0;

            Debug.Log(sceneController.ElementsSequence.Count);

            for (int i = 0; i < sceneController.ElementsSequence.Count; i ++)
            {
                PuzzleElementData puzzleElement = sceneController.ElementsSequence[i];
                Slot slot = snapBox.slots[i];

                if (slot != null && slot.IsFull() && slot.occupiedPuzzleElementData.groupName == puzzleElement.groupName) {
                    wellPlaced++;
                }
            }

            displayText.text = $"{wellPlaced}/{sceneController.ElementsSequence.Count}";

            if (wellPlaced == sceneController.ElementsSequence.Count)
            {
                Debug.Log("It fucking finished. Hurray!");
                var e = new TaskFinished();
                EventLogger.Instance.Log(e);
            }
        }
    }
}
