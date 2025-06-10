using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dades de l'element del puzzle.
/// </summary>
public class PuzzleElementData
{
    /// <summary>
    /// Nom del grup.
    /// </summary>
    public string groupName;

    /// <summary>
    /// Material de l'element.
    /// </summary>
    public Material material;

    /// <summary>
    /// Prefab de l'element.
    /// </summary>  
    public GameObject prefab;

    /// <summary>
    /// Parts del component.
    /// </summary>
    public List<PuzzleComponentData> componentParts = new List<PuzzleComponentData>();
}
