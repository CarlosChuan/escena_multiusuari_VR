using TMPro;
using UnityEngine;

public class SnapCounterDisplay : MonoBehaviour
{
    public SnapBox snapBox;
    public SceneController sceneController;
    public TMP_Text displayText;


    void OnEnable()
    {
        if (snapBox!= null)
        {
            snapBox.OnSnapChange += UpdateText;
        }
    }

    void OnDisable()
    {
        if (snapBox != null)
        {
            snapBox.OnSnapChange -= UpdateText;
        }
    }

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
