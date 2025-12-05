using UnityEngine;

public interface IDataPersistance
{
    void LoadData(GameData data, string levelId);
    void SaveData(ref GameData data, string levelId);
}
