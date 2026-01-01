using UnityEngine;

public class MovableObjectData : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    void Start()
    {
        if (string.IsNullOrEmpty(id))
        {
            GenerateGuid();
        }    
    }

    public void LoadData(GameData data, string levelId)
    {
        if (data.levels[levelId].objectsPositions.TryGetValue(id, out var pos))
        {
            transform.position = pos;
        }
    }

    public void SaveData(ref GameData data, string levelId)
    {

        if (data.levels[levelId].objectsPositions.ContainsKey(id))
        {
            data.levels[levelId].objectsPositions.Remove(id);
        }
        data.levels[levelId].objectsPositions.Add(id, transform.position);
    }
}
