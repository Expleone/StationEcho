using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class DataPersitanceManager : MonoBehaviour
{
    // [Header("Debugging")]
    // [SerializeField] private bool initializeDataIfNull = false;
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private string[] scenesIgnore;
    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;
    private bool isNewGame = false;

    public static DataPersitanceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Only one Data Persitsance Manager can exist in a scene.");
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesIgnore != null && scenesIgnore.Contains(scene.name))
        {
            Debug.Log("HEREEEE");
            return;
        }
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        Debug.Log("Found data persistance objects: " + dataPersistanceObjects.Count);
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scenesIgnore != null && scenesIgnore.Contains(scene.name)) return;
        Debug.Log("OnSceneUnloaded called");
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        this.isNewGame = true;
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game");

        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.LogWarning("No save data found. Creating new game data.");
            NewGame();
        }

        if (isNewGame)
        {
            Debug.Log("New Game: Scanning scene for default positions...");

            foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
            {
                dataPersistanceObj.SaveData(ref gameData);
            }
            isNewGame = false;
            SaveGame();
            return;
        }

        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found.New Game needs to be started");
            return;
        }

        Debug.Log("Saving Game");
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
