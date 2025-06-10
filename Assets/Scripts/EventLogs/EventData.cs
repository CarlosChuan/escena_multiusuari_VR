using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Dades de l'esdeveniment.
/// </summary>
[Serializable]
public class EventData
{
    public EventType type;
    public string name;
    public string timestamp;
    public string payload;  // store payload as JSON for flexibility
    public string from;
}

/// <summary>
/// Inicia una tasca.
/// </summary>
public class TaskStarted : EventData
{
    public TaskStarted(SceneType sceneType)
    {
        type = EventType.TaskStarted;
        name = type.ToString();
        payload = "sceneType: " + sceneType.ToString();
    }
}

/// <summary>
/// Finalitza una tasca.
/// </summary>
public class TaskFinished : EventData
{
    public TaskFinished()
    {
        type = EventType.TaskFinished;
        name = type.ToString();
    }
}

/// <summary>
/// Objecte agafat.
/// </summary>
public class ObjectPicked : EventData
{
    public ObjectPicked(string objectName)
    {
        type = EventType.ObjectPicked;
        name = type.ToString();
        payload = "object: " + objectName;
    }
}

/// <summary>
/// Objecte col·locat.
/// </summary>
public class ObjectPlaced : EventData
{
    public ObjectPlaced(string objectName, int slotTarget)
    {
        type = EventType.ObjectPlaced;
        name = type.ToString();
        payload = "object: " + objectName + ";target: " + slotTarget;
    }
}

/// <summary>
/// Error de col·locació.
/// </summary>
public class ErrorPlacement: EventData
{
    public ErrorPlacement(int slotTarget)
    {
        type = EventType.ErrorPlacement;
        name = type.ToString();
        payload = "target: " + slotTarget;
    }
}