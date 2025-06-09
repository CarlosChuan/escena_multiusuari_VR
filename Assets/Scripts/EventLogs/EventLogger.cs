using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    public static EventLogger Instance { get; private set; }

    private readonly List<EventData> _events = new();

    [SerializeField]
    private NetworkManager networkManager;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (networkManager == null)
        {
            networkManager = FindObjectOfType<NetworkManager>();
        }
    }

    public void Log(EventData e)
    {
        e.timestamp = System.DateTime.Now.ToString("o");
        e.name = e.type.ToString();
        e.from = networkManager.clientNickname();
        Debug.Log($"Logging {JsonUtility.ToJson(e, prettyPrint: true)}");
        _events.Add(e);
    }

    /// <summary>
    /// Call this at end of session to write all events out as JSON.
    /// </summary>
    public void FlushToDisk()
    {
        string json = JsonUtility.ToJson(new Wrapper<EventData>(_events), prettyPrint: true);
        string file = Path.Combine(Application.persistentDataPath,
                                  $"session_{System.DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(file, json);
        Debug.Log($"Events saved to {file}");
    }

    private void OnApplicationQuit()
    {
        FlushToDisk();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FlushToDisk();
        }
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
        public Wrapper(List<T> list) { items = list; }
    }
}
