using UnityEngine;

public class MovableObjectData : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;

    public Dispenser dispenser = null;



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
        print("Loading cube with ID " + id);
        if (data.levels[levelId].objectsPositions.TryGetValue(id, out var pos))
        {
            data.levels[levelId].objectsDispensors.TryGetValue(id, out dispenser);
            if (dispenser)
            {
                dispenser.DispenseItemAt(id, pos);
            }
            else
            {
                transform.position = pos;
            }
        }
    }

    public void SaveData(ref GameData data, string levelId)
    {

        if (data.levels[levelId].objectsPositions.ContainsKey(id))
        {
            data.levels[levelId].objectsPositions.Remove(id);
        }
        data.levels[levelId].objectsPositions.Add(id, transform.position);

        if (dispenser != null)
        {
            if (data.levels[levelId].objectsDispensors.ContainsKey(id))
            {
                data.levels[levelId].objectsDispensors.Remove(id);
            }
            data.levels[levelId].objectsDispensors.Add(id, dispenser);
        }
    }

    public void SetDispenser(Dispenser dispenser)
    {
        print("Got dispenser");
        this.dispenser = dispenser;
    }

    public string GetId()
    {
        return id;
    }

    public void SetId(string id)
    {
        this.id = id;
    }
}
