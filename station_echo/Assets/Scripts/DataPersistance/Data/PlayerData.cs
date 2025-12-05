using UnityEngine;


public class PlayerData : MonoBehaviour, IDataPersistance
{
    // Camera saving doesn't work properly
    private Camera playerCamera;


    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.Log("PlayerData: No Camera found in children of Player");
        }
    }

    public void LoadData(GameData data, string levelId)
    {
        transform.position = data.levels[levelId].playerPosition;
        // playerCamera.transform.position = data.cameraPosition;
        // playerCamera.transform.rotation = data.cameraRotation;
    }

    public void SaveData(ref GameData data, string levelId)
    {
        data.levels[levelId].playerPosition = transform.position;
        // data.cameraPosition = playerCamera.transform.position;
        // data.cameraRotation = playerCamera.transform.rotation;
    }
}
