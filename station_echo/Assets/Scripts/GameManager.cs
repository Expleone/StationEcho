using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Menu:
                PauseGame();
                break;
            case GameState.EscMenu:
                PauseGame();
                break;
            case GameState.Game:
                ResumeGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void SetMenuState() { UpdateGameState(GameState.Menu); }

    public void SetEscMenuState() { UpdateGameState(GameState.EscMenu); }

    public void SetGameState() { UpdateGameState(GameState.Game); }

}
public enum GameState
{
    Menu,
    EscMenu,
    Game
}
