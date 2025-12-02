using UnityEngine;

public class MovableObjectData : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data)
    {
        if (data.objectsPositions.TryGetValue(id, out var pos))
        {
            transform.position = pos;
        }
    }

    public void SaveData(ref GameData data)
    {

        if (data.objectsPositions.ContainsKey(id))
        {
            data.objectsPositions.Remove(id);
        }
        data.objectsPositions.Add(id, transform.position);
    }
}
