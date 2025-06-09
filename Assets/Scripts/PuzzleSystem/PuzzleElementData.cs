using System.Collections.Generic;
using UnityEngine;

public class PuzzleElementData
{
    public string groupName;
    public Material material;
    public GameObject prefab;
    public List<PuzzleComponentData> componentParts = new List<PuzzleComponentData>();
}
