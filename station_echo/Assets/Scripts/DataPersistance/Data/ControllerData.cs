using UnityEngine;

public class ControllerData : MonoBehaviour, IDataPersistance
{
    private Controller controllerComponent;
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        controllerComponent = GetComponent<Controller>();
        if (controllerComponent == null) Debug.LogError("ControllerData: No Controller component found on this GameObject!");

        if (string.IsNullOrEmpty(id)) GenerateGuid();
    }


    public void LoadData(GameData data, string levelId)
    {
        if (data.levels[levelId].controllerGravityChanged.TryGetValue(id, out var IsChanged))
        {
            controllerComponent.gravityChanged = IsChanged;
            // controllerComponent.SendMessage("ApplyState", IsCha, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SaveData(ref GameData data, string levelId)
    {
        if (data.levels[levelId].controllerGravityChanged.ContainsKey(id))
        {
            data.levels[levelId].controllerGravityChanged.Remove(id);
        }
        data.levels[levelId].controllerGravityChanged.Add(id, controllerComponent.gravityChanged);
    }
}
