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
    public bool isCompleted = false;
}
public class GameData
{
    public string currentLevel = "TutorialLevel";
    public Dictionary<string, Vector3> extraPositions = new Dictionary<string, Vector3>();
    public Dictionary<string, int> currentPlatformWaypoints = new Dictionary<string, int>();
   
    // public Vector3 playerPosition = new Vector3();
    // public Vector3 cameraPosition = new Vector3();
    // public Quaternion cameraRotation = new Quaternion();
    // public SerializableDictionary<string, Vector3> objectsPositions = new();
    public SerializableDictionary<string, LevelData> levels = new SerializableDictionary<string, LevelData>();

}
