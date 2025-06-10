using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

/// <summary>
/// Col·loca els elements en la taula.
/// </summary>
public class TablePlacer : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    [SerializeField] private Transform table;
    [SerializeField] private XRInteractionManager interactionManager;

    /// <summary>
    /// Llista de components de les peces.
    /// </summary>
    private List<GameObject> puzzleComponents;

    /// <summary>
    /// Llista de dades de les peces.
    /// </summary>
    public List<PuzzleComponentData> publicElementsData;

    /// <summary>
    /// Quan es fa start.
    /// </summary>
    private void OnStart()
    {
        if (interactionManager == null)
            this.interactionManager = FindObjectOfType<XRInteractionManager>();

        if (table == null)
            this.table = this.gameObject.transform;
    }

    /// <summary>
    /// Quan està habilitat.
    /// </summary>
    void OnEnable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated += PlaceElementsOnTable;
            sceneController.OnElementsRemoved += RemoveElementsFromTable;
        }
    }

    /// <summary>
    /// Quan està desabilitat.
    /// </summary>
    void OnDisable()
    {
        if (sceneController != null)
        {
            sceneController.OnElementsGenerated -= PlaceElementsOnTable;
            sceneController.OnElementsRemoved -= RemoveElementsFromTable;
        }
    }

    /// <summary>
    /// Quan es col·loca els elements en la taula.
    /// </summary>
    void PlaceElementsOnTable()
    {
        puzzleComponents = new();
        publicElementsData = new();
        var puzzleElementsData = new List<PuzzleElementData>(sceneController.ElementsSequence);
        if (puzzleElementsData == null || puzzleElementsData.Count == 0) return;

        foreach (var puzzleElementData in puzzleElementsData)
        {
            foreach (var puzzleComponentData in puzzleElementData.componentParts)
            {
                publicElementsData.Add(puzzleComponentData);
            }
        }


        // Creaci�n del generador aleatorio determinista con semilla
        System.Random rnd = new System.Random(sceneController.randomSeed);

        // Step 2: Shuffle the parts randomly
        Shuffle(publicElementsData, rnd);

        // Step 3: Calculate spread positions
        int partCount = publicElementsData.Count;
        if (partCount == 0) return;

        Vector3 tableCenter = table.position;
        Vector3 tableScale = table.localScale;

        Vector3 diff = new Vector3(
            (tableScale.x / 2) - 0.5f, // margin from edges
            0f,
            0f
        );

        Vector3 origin = tableCenter - diff;
        Vector3 end = tableCenter + diff;

        float step = (end.x - origin.x) / Mathf.Max(1, partCount - 1);

        // Step 4: Instantiate and place all parts
        for (int i = 0; i < partCount; i++)
        {
            Vector3 position = origin + new Vector3(i * step, 0.5f, 0f);

            GameObject instance = Instantiate(publicElementsData[i].prefab, position, Quaternion.identity);
            
            if (instance.TryGetComponent<PuzzleComponent>(out var puzzleComponent))
            {
                puzzleComponent.groupName = publicElementsData[i].groupName;
            } else
            {
                puzzleComponent = instance.AddComponent<PuzzleComponent>();
                puzzleComponent.groupName = publicElementsData[i].groupName;
                puzzleComponent.order = publicElementsData[i].order;
            }

            instance.tag = "Snappable";

            Renderer renderer = instance.GetComponent<Renderer>();
            renderer.material = publicElementsData[i].material;

            // Initializing photonView and photonViewController
            PhotonView photonView = instance.AddComponent<PhotonView>();
            Debug.Log("PhotonView setted");
            photonView.ViewID = 100 + i;
            Debug.Log($"ViewID is: {photonView.ViewID}");
            photonView.OwnershipTransfer = OwnershipOption.Request;

            PhotonTransformViewClassic photonTransform = instance.AddComponent<PhotonTransformViewClassic>();

            Debug.Log($"PhotonTransform added {photonTransform != null}");

            photonTransform.m_PositionModel.SynchronizeEnabled = true;
            photonTransform.m_RotationModel.SynchronizeEnabled = true;
            if (photonView.ObservedComponents == null)
                photonView.ObservedComponents = new List<Component>();
            photonView.ObservedComponents.Add(photonTransform);
            
            if (!instance.TryGetComponent<NetworkInteractableObject>(out var interactable))
            {
                interactable = instance.AddComponent<NetworkInteractableObject>();
                Debug.Log("NetworkInteractableObject added properly");
                interactable.interactionManager = interactionManager;
            }

            puzzleComponents.Add(instance);
        }
    }

    /// <summary>
    /// Quan es treuen els elements de la taula.
    /// </summary>
    void RemoveElementsFromTable()
    {
        if (puzzleComponents != null)
        {
            foreach (GameObject puzzleComponent in puzzleComponents)
            {
                Destroy(puzzleComponent);
            }
        }
    }

    /// <summary>
    /// Barreges una llista.
    /// </summary>
    /// <typeparam name="T">Tipus de la llista.</typeparam>
    /// <param name="list">Llista a barreger.</param>
    /// <param name="rnd">Generador aleatori.</param>
    void Shuffle<T>(List<T> list, System.Random rnd)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = rnd.Next(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    /// <summary>
    /// Crea un objecte remot per tal de sincronitzar-ho.
    /// </summary>
    /// <param name="position">Posició.</param>
    /// <param name="rotation">Rotació.</param>
    /// <param name="ViewID">ID de la vista.</param>
    /// <param name="indexInElements">Índex de l'element.</param>
    public void CreateRemoteObject (Vector3 position, Quaternion rotation, int ViewID, int indexInElements)
    {
        Debug.Log($"Creating remote object {indexInElements} in {position} with ID {ViewID}");

        GameObject instance = Instantiate(publicElementsData[indexInElements].prefab, position, rotation);
        PhotonView photonView = instance.AddComponent<PhotonView>();
        photonView.ViewID = ViewID;

        PhotonTransformViewClassic photonTransform = instance.AddComponent<PhotonTransformViewClassic>();
        photonTransform.m_PositionModel.SynchronizeEnabled = true;
        photonTransform.m_RotationModel.SynchronizeEnabled = true;
        if (photonView.ObservedComponents == null)
            photonView.ObservedComponents = new List<Component>();
        photonView.ObservedComponents.Add(photonTransform);
    }
}
