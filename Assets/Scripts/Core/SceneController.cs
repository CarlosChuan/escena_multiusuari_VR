using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum SceneType
{
    Tutorial,
    UShape,
    BehindStart,
    BehindEnd
}

public struct SceneTransformData
{
    public Vector3 position;
    public Quaternion rotation;

    public SceneTransformData(Vector3 pos, Vector3 rotEuler)
    {
        position = pos;
        rotation = Quaternion.Euler(rotEuler);
    }
}

public class SceneController : MonoBehaviour
{
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

    // This is the sequence other components can use
    private List<PuzzleElementData> elementDataList = new List<PuzzleElementData>();
    public IReadOnlyList<PuzzleElementData> ElementsSequence => elementDataList;

    public event System.Action OnElementsGenerated;
    public event System.Action OnElementsRemoved;

    public int randomSeed;

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


    public void GenerateRandomElements(SceneType sceneType, int seed)
    {
        // Move solution board to proper position
        SceneTransformData typeTransform = solutionBoardByType[sceneType];
        this.solutionBoard.transform.localPosition = typeTransform.position;
        this.solutionBoard.transform.localRotation = typeTransform.rotation;

        this.randomSeed = seed;

        elementDataList.Clear();

        // Creación del generador aleatorio determinista con semilla
        System.Random rnd = new System.Random(randomSeed);
        int numberOfElementsInSession = numberOfElements;
        // Build all possible combinations
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

        // Make sure we don't ask for more than what's possible
        if (numberOfElementsInSession > combinations.Count)
        {
            Debug.LogWarning("Requested number of elements exceeds available unique prefab-material combinations.");
            numberOfElementsInSession = combinations.Count;
        }

        // Shuffle combinations
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

           elementDataList.Add(puzzleElementData);
            Debug.Log($"Added element {groupName}");
        }

        OnElementsGenerated?.Invoke();

        var e = new TaskStarted(sceneType);
        EventLogger.Instance.Log(e);
    }

    void Shuffle<T>(List<T> list, System.Random rnd)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = rnd.Next(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void FinishScene()
    {
        Debug.Log("Finishing scene.");
        elementDataList.Clear();

        OnElementsRemoved?.Invoke();
    }

    public PuzzleElementData GetElementByGroupName(string groupName)
    {
        return elementDataList.FirstOrDefault(e => e.groupName == groupName);
    }
}
