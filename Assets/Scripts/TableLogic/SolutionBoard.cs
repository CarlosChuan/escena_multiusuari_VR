using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Taula de solució.
/// </summary>
public class SolutionBoard : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    [SerializeField] private Transform[] displaySlots;
    [SerializeField] private GameObject table;
    [SerializeField] private XRInteractionManager interactionManager;

    /// <summary>
    /// Espaiat entre slots.
    /// </summary>
    private float slotSpacing = -1;

    /// <summary>
    /// Desplaçament de la taula.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Total d'elements.
    /// </summary>
    private int totalElements;

    /// <summary>
    /// Quan es fa awake.
    /// </summary>
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

    /// <summary>
    /// Quan es fa draw gizmos.
    /// </summary>
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

            // Convert to world space using object�s rotation & scale
            Vector3 worldPosition = transform.TransformPoint(localOffset);

            Gizmos.DrawSphere(worldPosition, 0.1f);
        }

    }

    /// <summary>
    /// Genera l'offset i l'espaiat.
    /// </summary>
    public void GenerateOffsetAndSpacing()
    {
        // Obté la transformació de la taula.
        Transform tableTransform = table.transform;

        // Obté el centre de la taula.
        Vector3 tableCenter = tableTransform.position;

        // Obté el tamany de la taula.
        Vector3 tableScale = tableTransform.localScale;

        // Calcula la diferència entre el centre i el marge.
        Vector3 diff = new Vector3(
            (tableScale.x / 2) - 0.5f,
            0f,
            0f
        );

        // Obté el punt d'origen i el final.
        Vector3 origin = tableCenter - diff;
        Vector3 end = tableCenter + diff;

        // Obté el total d'elements.
        totalElements = sceneController.numberOfElements;

        // Calcula l'espaiat entre slots.
        slotSpacing = (end.x - origin.x) / Mathf.Max(1, totalElements - 1);

        // Calcula la mida del contingut.
        float contentWidth = (slotSpacing * totalElements) - slotSpacing;

        // Calcula un offset vertical.
        float yOffset = (tableScale.y / 2) - .5f;

        // Assigna el vector de desplaçament.
        offset = new Vector3(contentWidth / -2, yOffset, table.transform.localScale.z / 2);
    }

    /// <summary>
    /// Genera els slots a partir de la seqüència.
    /// </summary>
    public void GenerateSlotsFromSequence(int count)
    {
        // Destrueix els fills antics.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "Table") continue;
            Destroy(child);
        }

        // Si no hi ha dades o no hi ha elements.
        if (sceneController == null || sceneController.ElementsSequence == null)
        {
            Debug.LogWarning("SceneController or ElementsSequence is missing!");
            return;
        }

        // Si no hi ha espaiat.
        if (slotSpacing == -1) {
            GenerateOffsetAndSpacing();
        }

        // Inicialitza el vector de slots.
        displaySlots = new Transform[count];

        // Repeteix per cada slot.
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

    /// <summary>
    /// Quan està habilitat.
    /// </summary>
    void OnEnable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated += PopulateBoard;
            sceneController.OnElementsRemoved += RemoveBoard;
        }
    }

    /// <summary>
    /// Quan està desabilitat.
    /// </summary>
    void OnDisable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated -= PopulateBoard;
            sceneController.OnElementsRemoved -= RemoveBoard;
        }
    }

    /// <summary>
    /// Crea la taula amb tots els elements.
    /// </summary>
    private void PopulateBoard()
    {
        print("PopulateBoard -- START");
        
        // Obté les dades dels elements.
        var puzzleElementsData = new List<PuzzleElementData>(sceneController.ElementsSequence);
        if (puzzleElementsData == null || puzzleElementsData.Count == 0) return;

        print("PopulateBoard -- PuzzleElements " + puzzleElementsData.Count);

        // Genera els slots a partir de la seqüència.
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

    /// <summary>
    /// Elimina la taula.
    /// </summary>
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
