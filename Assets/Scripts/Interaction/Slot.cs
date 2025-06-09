using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public GameObject parentGameObject;
    public Transform slotTransform;
    public float snapRadius = 1f;
    public Dictionary<int, GameObject> orderGameObjects = new();
    public PuzzleElementData occupiedPuzzleElementData = null;



    public bool IsOccupied()
    {
        return this.occupiedPuzzleElementData != null;
    }

    public bool IsFull()
    {
        if (occupiedPuzzleElementData == null) return false;
        return this.orderGameObjects.ContainsKey(occupiedPuzzleElementData.componentParts.Count - 1);
    }

    public void ClearSlot()
    {
        this.occupiedPuzzleElementData = null;
        this.orderGameObjects.Clear();
    }
}
