using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slot.
/// </summary>
[System.Serializable]
public class Slot
{
    // Component de l'objecte.
    public GameObject parentGameObject;

    // Component de la posició.
    public Transform slotTransform;

    // Radi de col·lisió.
    public float snapRadius = 1f;

    // Llista d'objectes.
    public Dictionary<int, GameObject> orderGameObjects = new();
    public PuzzleElementData occupiedPuzzleElementData = null;

    /// <summary>
    /// Comprova si l'slot està ocupat.
    /// </summary>
    /// <returns>True si l'slot està ocupat, false en cas contrari.</returns>
    public bool IsOccupied()
    {
        return this.occupiedPuzzleElementData != null;
    }

    /// <summary>
    /// Comprova si l'slot està ple.
    /// </summary>
    /// <returns>True si l'slot està ple, false en cas contrari.</returns>
    public bool IsFull()
    {
        if (occupiedPuzzleElementData == null) return false;
        return this.orderGameObjects.ContainsKey(occupiedPuzzleElementData.componentParts.Count - 1);
    }

    /// <summary>
    /// Buida i neteja l'slot.
    /// </summary>
    public void ClearSlot()
    {
        this.occupiedPuzzleElementData = null;
        this.orderGameObjects.Clear();
    }
}
