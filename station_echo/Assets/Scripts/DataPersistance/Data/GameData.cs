using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]

public class LevelData
{
    public Vector3 playerPosition = new Vector3();
    public Vector3 cameraPosition = new Vector3();
    public Quaternion cameraRotation = new Quaternion();
    public SerializableDictionary<string, Vector3> objectsPositions = new SerializableDictionary<string, Vector3>();

    public SerializableDictionary<string, Dispenser> objectsDispensors = new SerializableDictionary<string, Dispenser>();
    public SerializableDictionary<string, bool> switchIsPressed = new SerializableDictionary<string, bool>();
    public SerializableDictionary<string, bool> controllerGravityChanged = new SerializableDictionary<string, bool>();
    public bool isCompleted = false;
    public Vector3 currentGravitation = Physics.gravity;
}
public class GameData
{
    public string currentLevel = "LVL0";
    public Dictionary<string, Vector3> extraPositions = new Dictionary<string, Vector3>();
    public Dictionary<string, int> currentPlatformWaypoints = new Dictionary<string, int>();

    // public Vector3 playerPosition = new Vector3();
    // public Vector3 cameraPosition = new Vector3();
    // public Quaternion cameraRotation = new Quaternion();
    // public SerializableDictionary<string, Vector3> objectsPositions = new();
    public SerializableDictionary<string, LevelData> levels = new SerializableDictionary<string, LevelData>();

}
