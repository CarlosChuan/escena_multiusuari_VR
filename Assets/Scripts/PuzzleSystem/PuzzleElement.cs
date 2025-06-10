using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Element del puzzle.
/// </summary>
public class PuzzleElement : MonoBehaviour
{
    [Header("Puzzle Element Settings")]
    public List<GameObject> componentParts;

    /// <summary>
    /// Auto-omple les parts vàlides del component.
    /// </summary>
    [ContextMenu("Auto-Fill Valid Component Parts")]
    public void AutoFillComponentParts()
    {
        componentParts.Clear();
        PuzzleComponent[] parts = GetComponentsInChildren<PuzzleComponent>();

        foreach (var part in parts)
            componentParts.Add(part.gameObject);
    }

    /// <summary>
    /// Quan es fa awake.
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < componentParts.Count; i++)
        {
            // Si la part no existeix o no té el component PuzzleComponent.
            if (!componentParts[i] || !componentParts[i].TryGetComponent<PuzzleComponent>(out _))
            {
                throw new System.Exception($"PuzzleElement '{gameObject.name}' has invalid child configuration.");
            }
        }
    }
}
