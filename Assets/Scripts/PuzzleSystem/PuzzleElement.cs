using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PuzzleElement : MonoBehaviour
{
    [Header("Puzzle Element Settings")]
    public List<GameObject> componentParts;


    [ContextMenu("Auto-Fill Valid Component Parts")]
    public void AutoFillComponentParts()
    {
        componentParts.Clear();
        PuzzleComponent[] parts = GetComponentsInChildren<PuzzleComponent>();

        foreach (var part in parts)
            componentParts.Add(part.gameObject);
    }

    private void Awake()
    {
        for (int i = 0; i < componentParts.Count; i++)
        {
            if (!componentParts[i] || !componentParts[i].TryGetComponent<PuzzleComponent>(out _))
            {
                throw new System.Exception($"PuzzleElement '{gameObject.name}' has invalid child configuration.");
            }
        }
    }
}
