using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [Header("Initial Scene")]
    [Tooltip("The scene to load ater initialization")]
    public string intitalSceneName = "Menu";

    [Header("Initial State")]
    [Tooltip("The state GameManager will assume upon loading")]
    public GameState initalGameManagerState = GameState.Menu;
    void Start()
    {
        if (GameManager.Instance == null)
        {
            GameObject gameManager = new GameObject("Game Manager");
            gameManager.AddComponent<GameManager>();
        }
        LoadInitialScene();
    }

    private void LoadInitialScene()
    {
        SceneManager.LoadScene(intitalSceneName);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(initalGameManagerState);
        }
    }
}
