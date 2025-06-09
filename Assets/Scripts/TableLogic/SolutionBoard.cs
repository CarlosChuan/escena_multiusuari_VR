using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

public class SolutionBoard : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    [SerializeField] private Transform[] displaySlots;
    [SerializeField] private GameObject table;
    [SerializeField] private XRInteractionManager interactionManager;

    private float slotSpacing = -1;
    private Vector3 offset;

    private int totalElements;

    private void Start()
    {
        if (interactionManager == null)
            this.interactionManager = FindObjectOfType<XRInteractionManager>();

        if (slotSpacing == -1)
        {
            // Positions declarations
            GenerateOffsetAndSpacing();
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;


        if (offset == null || slotSpacing == -1 || totalElements != sceneController.numberOfElements)
        {
            GenerateOffsetAndSpacing();
        }

        for (int i = 0; i < totalElements; i++)
        {
            // Local space position (relative to the object)
            Vector3 localOffset = offset + Vector3.right * i * slotSpacing;

            // Convert to world space using object’s rotation & scale
            Vector3 worldPosition = transform.TransformPoint(localOffset);

            Gizmos.DrawSphere(worldPosition, 0.1f);
        }

    }

    public void GenerateOffsetAndSpacing()
    {
        Transform tableTransform = table.transform;
        Vector3 tableCenter = tableTransform.position;
        Vector3 tableScale = tableTransform.localScale;

        Vector3 diff = new Vector3(
            (tableScale.x / 2) - 0.5f, // margin from edges
            0f,
            0f
        );

        Vector3 origin = tableCenter - diff;
        Vector3 end = tableCenter + diff;

        totalElements = sceneController.numberOfElements;

        slotSpacing = (end.x - origin.x) / Mathf.Max(1, totalElements - 1);

        float contentWidth = (slotSpacing * totalElements) - slotSpacing;

        float yOffset = (tableScale.y / 2) - .5f;

        offset = new Vector3(contentWidth / -2, yOffset, table.transform.localScale.z / 2);
    }

    public void GenerateSlotsFromSequence(int count)
    {
        // Destroy old children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "Table") continue;
            Destroy(child);
        }
        
        if (sceneController == null || sceneController.ElementsSequence == null)
        {
            Debug.LogWarning("SceneController or ElementsSequence is missing!");
            return;
        }

        if (slotSpacing == -1) {
            // Positions declarations
            GenerateOffsetAndSpacing();
        }

        displaySlots = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            GameObject slot = new GameObject($"Slot_{i}");
            slot.transform.parent = transform;

            Vector3 localPosition = offset + Vector3.right * i * slotSpacing;

            print("Creating Object. LocalPosition " + localPosition + " Offset: " + offset + " Spacing: " + slotSpacing);

            slot.transform.localPosition = localPosition;
            slot.transform.localRotation = Quaternion.identity;

            displaySlots[i] = slot.transform;
        }
    }

    void OnEnable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated += PopulateBoard;
            sceneController.OnElementsRemoved += RemoveBoard;
        }
    }

    void OnDisable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated -= PopulateBoard;
            sceneController.OnElementsRemoved -= RemoveBoard;
        }
    }

    private void PopulateBoard()
    {
        print("PopulateBoard -- START");
        
        var puzzleElementsData = new List<PuzzleElementData>(sceneController.ElementsSequence);
        if (puzzleElementsData == null || puzzleElementsData.Count == 0) return;

        print("PopulateBoard -- PuzzleElements " + puzzleElementsData.Count);

        GenerateSlotsFromSequence(puzzleElementsData.Count);

        for (int i = 0; i < puzzleElementsData.Count; i++)
        {
            GameObject gameObject = Instantiate(puzzleElementsData[i].prefab, displaySlots[i]);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = puzzleElementsData[i].material;
            }

            gameObject.name += "_board";

            XRGrabInteractable interactable = gameObject.GetComponent<XRGrabInteractable>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRGrabInteractable>();
                interactable.interactionManager = interactionManager;
            }

            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            rigidbody.mass = 1f;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            var returner = gameObject.AddComponent<ReturnToSlot>();
            returner.AssignSlot(displaySlots[i]);
        }
    }

    private void RemoveBoard()
    {
        if (displaySlots != null)
        {
            foreach (Transform displaySlot in displaySlots)
            {
                Destroy(displaySlot.gameObject);
            }

            displaySlots = null;
        }
    }
}
