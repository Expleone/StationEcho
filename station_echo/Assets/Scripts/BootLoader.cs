using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [Header("Initial Scene")]
    [Tooltip("The scene to load ater initialization")]
    public string intitalSceneName = "Menu";
    void Start()
    {
        if(GameManager.Instance == null)
        {
            GameObject gameManager = new GameObject("Game Manager");
            gameManager.AddComponent<GameManager>();
        }
        LoadInitialScene();
    }

    private void LoadInitialScene()
    {
        SceneManager.LoadScene(intitalSceneName);
    }
}
