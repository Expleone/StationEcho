// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class LevelData : MonoBehaviour, IDataPersistance
// {
//     public string levelId;
//     public Vector3 playerPosition = new Vector3();
//     public Vector3 cameraPosition = new Vector3();
//     public Quaternion cameraRotation = new Quaternion();
//     public SerializableDictionary<string, Vector3> objectsPositions = new SerializableDictionary<string, Vector3>();

//     void Start()
//     {
//         levelId = SceneManager.GetActiveScene().ToString();
//         Debug.Log("Scene Id: " + levelId);
//     }

//     public void LoadData(GameData data, string levelId)
//     {

//     }

//     public void SaveData(ref GameData data, string levelId)
//     {

//     }
// }
