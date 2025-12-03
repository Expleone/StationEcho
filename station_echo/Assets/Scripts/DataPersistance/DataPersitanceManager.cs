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
    [Header("Save game on exiting level/application")]
    public bool saveOnSceneUnloaded = true;
    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;
    private bool isNewGame = false;
    private string levelId;
    public static DataPersitanceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Only one Data Persitsance Manager can exist in a scene.");
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
        if (scenesIgnore != null && scenesIgnore.Contains(scene.name)) return;
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        levelId = SceneManager.GetActiveScene().name;
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scenesIgnore != null && scenesIgnore.Contains(scene.name)) return;
        levelId = SceneManager.GetActiveScene().name;
        if (saveOnSceneUnloaded) SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        this.isNewGame = true;
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No save data found. Creating new Game Data.");
            NewGame();
        }

        if (isNewGame || gameData.levels[levelId].isCompleted)
        {
            Debug.Log("New Game: Scanning scene for default positions...");

            levelId = SceneManager.GetActiveScene().name;
            foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
            {
                dataPersistanceObj.SaveData(ref gameData, levelId);
            }
            isNewGame = false;
            SaveGame();
            return;
        }

        CheckIfLevelDataExists();
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gameData, levelId);
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

        CheckIfLevelDataExists();
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gameData, levelId);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        if (saveOnSceneUnloaded) SaveGame();
    }

    private void CheckIfLevelDataExists()
    {
        string levelId = SceneManager.GetActiveScene().name;
        if (!gameData.levels.ContainsKey(levelId))
        {
            gameData.levels[levelId] = new LevelData();
            isNewGame = true;
        }
    }

    public void SetCurrentLevelAsCompleted()
    {
        gameData.levels[levelId].isCompleted = true;
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
