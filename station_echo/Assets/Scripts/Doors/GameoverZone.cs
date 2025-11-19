using UnityEditor;
using UnityEngine;


public class GameoverZone : MonoBehaviour
{

    private MenuManager menuManager;

    private void Start()
    {
        menuManager = Object.FindFirstObjectByType<MenuManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FinishGame();
        }
    }


    private void FinishGame()
    {
        // TODO: Make a Gameover Menu with an option to restart the level
        menuManager.LoadGameStateMenu();
    }
}
