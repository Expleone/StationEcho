using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Settings")]
    [Tooltip("Whether this menu can be collapsed with the Escape key")]
    public bool isCollapsable = true;
    [Tooltip("Whether menu is visble on start")]
    public bool isVisibleOnStart = true;

    public Canvas menuCanvas;

    void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.enabled = isVisibleOnStart;
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && isCollapsable)
        {
            if (menuCanvas.enabled)
            {
                SwitchGameStateToGame();
            }
            else
            {
                SwitchGameStateToEscMenu();
            }
        }
    }
    public void LoadGameStateMenu()
    {
        DisableMenu();
        // DataPersitanceManager.instance.SaveGame();
        SceneManager.LoadScene("Menu");
        GameManager.Instance.UpdateGameState(GameState.Menu);
    }

    public void SwitchGameStateToEscMenu()
    {
        EnableMenu();
        GameManager.Instance.UpdateGameState(GameState.EscMenu);
    }

    public void LoadGameStateGame(string sceneName)
    {
        DisableMenu();
        GameManager.Instance.UpdateGameState(GameState.Game);
        SceneManager.LoadScene(sceneName);
    }

    public void NewGameStateGame(string sceneName)
    {
        DisableMenu();
        GameManager.Instance.UpdateGameState(GameState.Game);
        DataPersitanceManager.instance.NewGame();
        SceneManager.LoadScene(sceneName);

    }

    public void SwitchGameStateToGame()
    {
        DisableMenu();
        GameManager.Instance.UpdateGameState(GameState.Game);
    }

    public void EnableMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.enabled = true;
        }
    }

    public void DisableMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.enabled = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}