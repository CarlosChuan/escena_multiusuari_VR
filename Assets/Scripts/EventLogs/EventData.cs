using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class EventData
{
    public EventType type;
    public string name;
    public string timestamp;
    public string payload;  // store payload as JSON for flexibility
    public string from;
}

public class TaskStarted : EventData
{
    public TaskStarted(SceneType sceneType)
    {
        type = EventType.TaskStarted;
        name = type.ToString();
        payload = "sceneType: " + sceneType.ToString();
    }
}

public class TaskFinished : EventData
{
    public TaskFinished()
    {
        type = EventType.TaskFinished;
        name = type.ToString();
    }
}

public class ObjectPicked : EventData
{
    public ObjectPicked(string objectName)
    {
        type = EventType.ObjectPicked;
        name = type.ToString();
        payload = "object: " + objectName;
    }
}

public class ObjectPlaced : EventData
{
    public ObjectPlaced(string objectName, int slotTarget)
    {
        type = EventType.ObjectPlaced;
        name = type.ToString();
        payload = "object: " + objectName + ";target: " + slotTarget;
    }
}

public class ErrorPlacement: EventData
{
    public ErrorPlacement(int slotTarget)
    {
        type = EventType.ErrorPlacement;
        name = type.ToString();
        payload = "target: " + slotTarget;
    }
}