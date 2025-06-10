using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Gestiona el registre d'esdeveniments.
/// </summary>
public class EventLogger : MonoBehaviour
{
    // Instància única.
    public static EventLogger Instance { get; private set; }

    // Llista d'esdeveniments.
    private readonly List<EventData> _events = new();

    // Component de xarxa.
    [SerializeField]
    private NetworkManager networkManager;

    // Inicialització.
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

    /// <summary>
    /// Registra un esdeveniment.
    /// </summary>
    /// <param name="e">Esdeveniment.</param>
    public void Log(EventData e)
    {
        e.timestamp = System.DateTime.Now.ToString("o");
        e.name = e.type.ToString();
        e.from = networkManager.clientNickname();
        Debug.Log($"Logging {JsonUtility.ToJson(e, prettyPrint: true)}");
        _events.Add(e);
    }

    /// <summary>
    /// Escriu tots els esdeveniments en format JSON.
    /// </summary>
    public void FlushToDisk()
    {
        string json = JsonUtility.ToJson(new Wrapper<EventData>(_events), prettyPrint: true);
        string file = Path.Combine(Application.persistentDataPath,
                                  $"session_{System.DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(file, json);
        Debug.Log($"Events saved to {file}");
    }

    /// <summary>
    /// Quan l'aplicació es tanca.
    /// </summary>
    private void OnApplicationQuit()
    {
        FlushToDisk();
    }

    /// <summary>
    /// Quan l'aplicació es para.
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FlushToDisk();
        }
    }

    /// <summary>
    /// Wrapper per a la llista d'esdeveniments.
    /// </summary>
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
        public Wrapper(List<T> list) { items = list; }
    }
}
