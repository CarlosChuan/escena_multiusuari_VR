﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Caixa de col·lisió.
/// </summary>  
public class SnapBox : MonoBehaviour
{
    [SerializeField] public List<Slot> slots;
    [SerializeField] public GameObject slotPrefab;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameObject table;

    // Event de canvi de col·lisió.
    public event System.Action OnSnapChange;

    /// <summary>
    /// Quan està habilitat.
    /// </summary>
    void OnEnable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated += AssignSlots;
            sceneController.OnElementsRemoved += RemoveSlots;
        }
    }

    /// <summary>
    /// Quan està deshabilitat.
    /// </summary>
    void OnDisable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated -= AssignSlots;
            sceneController.OnElementsRemoved -= RemoveSlots;
        }
    }

    /// <summary>
    /// Assigna els slots.
    /// </summary>
    public void AssignSlots()
    {
        Transform thisTransform = table.transform;
        Vector3 tableCenter = thisTransform.position;
        Vector3 tableScale = thisTransform.localScale;

        Vector3 diff = new Vector3(
            (tableScale.x / 2) - 0.5f, // margin from edges
            0f,
            0f
        );

        Vector3 origin = tableCenter - diff;
        Vector3 end = tableCenter + diff;

        float step = (end.x - origin.x) / Mathf.Max(1, sceneController.numberOfElements - 1);

        print("Step " + step);

        for (int i = 0; i < sceneController.numberOfElements; i++)
        {
            Vector3 position = origin + new Vector3(i * step, 0.2f, 0f);

            print("Position " + position);

            GameObject instance = Instantiate(slotPrefab, position, Quaternion.identity, this.transform);

            Slot slot = new()
            {
                parentGameObject = instance,
                slotTransform = instance.transform.GetChild(0).transform
            };

            if (slot.slotTransform.TryGetComponent<SnapSlot>(out var trigger))
            {
                trigger.snapBox = this;
                trigger.slotIndex = i;
            }

            this.slots.Add(slot);
        }

        OnSnapChange?.Invoke();
    }

    /// <summary>
    /// Elimina els slots.
    /// </summary>
    private void RemoveSlots()
    {
        Debug.Log("Removing slots in SnapBox");
        if (slots != null)
        {
            foreach (Slot slot in slots)
            {
                if (slot.parentGameObject != null)
                {
                    Destroy(slot.parentGameObject);
                }
            }
             
            slots.Clear();
        }

        OnSnapChange?.Invoke();
    }

    /// <summary>
    /// Intenta col·locar un objecte en un slot.
    /// </summary>
    /// <param name="obj">Objecte a col·locar.</param>
    /// <param name="slotIndex">Índex del slot.</param>
    /// <returns>True si l'objecte s'ha col·locat, false en cas contrari.</returns>
    public bool TrySnap(GameObject obj, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return false;

        Slot slot = slots[slotIndex];

        PuzzleComponent puzzleComponent = obj.GetComponent<PuzzleComponent>();
        PuzzleElementData puzzleElementData = sceneController.GetElementByGroupName(puzzleComponent.groupName);

        if (!slot.IsOccupied())
        {
            slot.occupiedPuzzleElementData = puzzleElementData;
        } else if (slot.occupiedPuzzleElementData.groupName != puzzleComponent.groupName) 
        {
            Debug.Log($"❌ Cannot place {puzzleComponent.name}: trying to attach different group object to it!.");
            return false;
        } else if (slot.IsFull())
        {
            Debug.Log($"❌ Cannot place {puzzleComponent.name}: already filled.");
            return false;
        }

        var groupDict = slot.orderGameObjects;

        bool isBase = puzzleComponent.order == 0;
        bool previousPlaced = groupDict.ContainsKey(puzzleComponent.order - 1);

        if (!isBase && !previousPlaced)
        {
            Debug.Log($"❌ Cannot place {puzzleComponent.name}: previous component not placed.");
            return false;
        }

        Vector3 basePosition = slot.slotTransform.position;
        basePosition.y += 0.5f;

        if (isBase)
        {
            // obj.transform.SetPositionAndRotation(basePosition, Quaternion.identity);
            // obj.transform.SetParent(slot.slotTransform);
        } else
        {
            //  GameObject belowObject = groupDict[puzzleComponent.order - 1];

            // Get height of the object we are stacking on
            // Bounds belowBounds = GetRendererBounds(belowObject);
            // float topY = belowBounds.max.y;

            // Bounds thisBounds = GetRendererBounds(obj);
            // float halfHeight = thisBounds.extents.y;

            // Vector3 targetPos = new Vector3(
            //     belowObject.transform.position.x,
            // topY + halfHeight,
            // belowObject.transform.position.z
            // );

            // obj.transform.SetPositionAndRotation(targetPos, belowObject.transform.rotation);
            // obj.transform.SetParent(belowObject.transform);
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        groupDict[puzzleComponent.order] = obj;

        OnSnapChange?.Invoke();

        return true;
    }

    /// <summary>
    /// Intenta descol·locar un objecte d'un slot.
    /// </summary>
    /// <param name="obj">Objecte a descol·locar.</param>
    /// <param name="slotIndex">Índex del slot.</param>
    /// <returns>True si l'objecte s'ha descol·locat, false en cas contrari.</returns>
    public bool TryDetach(GameObject obj, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return false;

        Slot slot = slots[slotIndex];
        if (!slot.IsOccupied())
            return false;

        PuzzleComponent puzzleComponent = obj.GetComponent<PuzzleComponent>();
        if (puzzleComponent == null)
            return false;

        var groupDict = slot.orderGameObjects;

        // Comprova si l'objecte existeix en el stack de l'slot.
        if (!groupDict.ContainsKey(puzzleComponent.order))
        {
            Debug.LogWarning($"❌ Cannot detach: object with order {puzzleComponent.order} not found in slot.");
            return false;
        }

        // Elimina l'objecte del stack.
        groupDict.Remove(puzzleComponent.order);

        // Desacopla l'objecte de la jerarquia.
        // obj.transform.SetParent(null);

        // Si aquest era l'últim element en el grup, neteja les dades de l'slot.
        if (groupDict.Count == 0)
        {
            slot.ClearSlot();
        }

        OnSnapChange?.Invoke();

        return true;
    }

    /// <summary>
    /// Obté el límit de l'objecte.
    /// </summary>
    /// <param name="obj">Objecte a comprovar.</param>
    /// <returns>Límit de l'objecte.</returns>
    private Bounds GetRendererBounds(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
            return rend.bounds;

        // Casos de fallada.
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            return bounds;
        }

        return new Bounds(obj.transform.position, Vector3.zero);
    }
}
