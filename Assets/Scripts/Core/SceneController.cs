using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tipus de distribucions.
/// </summary>
public enum SceneType
{
    Tutorial,
    UShape,
    BehindStart,
    BehindEnd
}

/// <summary>
/// Dades de transformació de la distribució.
/// </summary>
public struct SceneTransformData
{
    public Vector3 position;
    public Quaternion rotation;

    /// <summary>
    /// Constructor de dades de transformació.
    /// </summary>
    /// <param name="pos">Posició de la distribució.</param>
    /// <param name="rotEuler">Rotació de la distribució en graus.</param>
    public SceneTransformData(Vector3 pos, Vector3 rotEuler)
    {
        position = pos;
        rotation = Quaternion.Euler(rotEuler);
    }
}

/// <summary>
/// Controlador de la escena.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Elements de configuració de la escena, com llistat de materials i prefabs o llistat d'elements usats al tutorial.
    // Tots aquests han de ser introduits desde l'inspector de Unity.
    [Header("Elements Configuration")]
    [SerializeField] private List<Material> materials;
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField, Range(1, 10)]
    public int numberOfElements = 5;

    [Header("Tutorial Elements")]
    [SerializeField] private List<Material> tutorialMaterials;
    [SerializeField] private List<GameObject> tutorialPrefabs;

    [Header("Sesion Configuration")]
    [SerializeField] private GameObject solutionBoard;

    // Aquesta és la seqüència d'elements que altres components poden utilitzar.
    private List<PuzzleElementData> elementDataList = new List<PuzzleElementData>();
    public IReadOnlyList<PuzzleElementData> ElementsSequence => elementDataList;

    // Esdeveniments interns de Unity.
    public event System.Action OnElementsGenerated;
    public event System.Action OnElementsRemoved;

    public int randomSeed;

    // Diccionari amb la posició de la taula amb ordre objectiu segons tipus d'escena.
    private Dictionary<SceneType, SceneTransformData> solutionBoardByType = new()
    {
        { SceneType.Tutorial,  new SceneTransformData(
            new Vector3(-5.5f, 1f, 0f),
            new Vector3(0f, 90f, 0f)
        )},
        { SceneType.UShape,  new SceneTransformData(
            new Vector3(-5.5f, 1f, 0f),
            new Vector3(0f, 90f, 0f)
        )},
        { SceneType.BehindStart,  new SceneTransformData(
            new Vector3(0f, 1f, -5.5f),
            new Vector3(0f, 0f, 0f)
        )},
        { SceneType.BehindEnd,  new SceneTransformData(
            new Vector3(0f, 1f, 5.5f),
            new Vector3(0f, 180f, 0f)
        )},
    };

    /// <summary>
    /// Genera elements aleatoris segons el tipus de distribució i la llavor.
    /// </summary>
    /// <param name="sceneType">Tipus de distribució.</param>
    /// <param name="seed">Llavor.</param>
    public void GenerateRandomElements(SceneType sceneType, int seed)
    {
        // Mou la taula de solució a la posició adequada.
        SceneTransformData typeTransform = solutionBoardByType[sceneType];
        this.solutionBoard.transform.localPosition = typeTransform.position;
        this.solutionBoard.transform.localRotation = typeTransform.rotation;

        this.randomSeed = seed;

        elementDataList.Clear();

        // Creació del generador aleatori determinat amb la semilla.
        System.Random rnd = new System.Random(randomSeed);
        int numberOfElementsInSession = numberOfElements;

        // Construcció de totes les combinacions possibles.
        List<(GameObject prefab, Material material)> combinations = new List<(GameObject, Material)>();
        if (sceneType == SceneType.Tutorial)
        {
            for (int i = 0; i < tutorialPrefabs.Count; i++)
            {
                int materialIndex = i % tutorialMaterials.Count;
                Debug.Log($"Material Index {materialIndex}. i: {i}. tutoMaterials.count: {tutorialMaterials.Count}");
                combinations.Add((tutorialPrefabs[i], tutorialMaterials[materialIndex]));
            }

            numberOfElementsInSession = tutorialPrefabs.Count;
        } else
        {
            foreach (var prefab in prefabs)
            {
                foreach (var mat in materials)
                {
                    combinations.Add((prefab, mat));
                }
            }
        }

        // Assegura't que no demanis més que el que és possible.
        if (numberOfElementsInSession > combinations.Count)
        {
            Debug.LogWarning("Requested number of elements exceeds available unique prefab-material combinations.");
            numberOfElementsInSession = combinations.Count;
        }

        // Barreges les combinacions.
        Shuffle(combinations, rnd);

        for (int i = 0; i < numberOfElementsInSession; i++)
        {
            (GameObject prefab, Material material) combination = combinations[i];
            string groupName = combination.prefab.name + '_' + combination.material.name;
            PuzzleElementData puzzleElementData = new()
            {
                groupName = groupName,
                material = combination.material,
                prefab = combination.prefab
            };

            // Obté el component PuzzleElement del prefab.
            PuzzleElement logic = combination.prefab.GetComponent<PuzzleElement>();
            foreach (var component in logic.componentParts)
            {
                PuzzleComponent componentLogic = component.GetComponent<PuzzleComponent>();
                print(componentLogic + " " + component.name + " " + combination.material.name + " " + groupName + " " + componentLogic.order);
                PuzzleComponentData componentData = new()
                {
                    prefab = component,
                    material = combination.material,
                    groupName = groupName,
                    order = componentLogic.order,
                };
                puzzleElementData.componentParts.Add(componentData);
            };

            // Afegeix l'element a la llista.
            elementDataList.Add(puzzleElementData);
            Debug.Log($"Added element {groupName}");
        }

        // Invoca l'esdeveniment de generació d'elements.
        OnElementsGenerated?.Invoke();

        // Inicia el registre d'esdeveniments.
        var e = new TaskStarted(sceneType);
        EventLogger.Instance.Log(e);
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
    /// Finalitza la escena.
    /// </summary>
    public void FinishScene()
    {
        Debug.Log("Finishing scene.");
        elementDataList.Clear();

        OnElementsRemoved?.Invoke();
    }

    /// <summary>
    /// Obté un element de la llista segons el nom del grup.
    /// </summary>
    /// <param name="groupName">Nom del grup.</param>
    /// <returns>Element de la llista.</returns>
    public PuzzleElementData GetElementByGroupName(string groupName)
    {
        return elementDataList.FirstOrDefault(e => e.groupName == groupName);
    }
}
