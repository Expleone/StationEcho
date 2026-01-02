using UnityEngine;

public class SwitchData : MonoBehaviour, IDataPersistance
{
    private Switch switchComponent;

    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        switchComponent = GetComponent<Switch>();
        if (switchComponent == null) Debug.LogError("SwitchDataPersistance: No Switch component found on this GameObject!");

        if (string.IsNullOrEmpty(id)) GenerateGuid();
    }

    public void LoadData(GameData data, string levelId)
    {
        if (data.levels[levelId].switchIsPressed.TryGetValue(id, out var IsPressed))
        {
            switchComponent.IsOn = IsPressed;
            StartCoroutine(switchComponent.ApplyState());
            // switchComponent.SendMessage("ApplyState", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SaveData(ref GameData data, string levelId)
    {
        if (data.levels[levelId].switchIsPressed.ContainsKey(id))
        {
            data.levels[levelId].switchIsPressed.Remove(id);
        }
        data.levels[levelId].switchIsPressed.Add(id, switchComponent.IsOn);

    }
}
