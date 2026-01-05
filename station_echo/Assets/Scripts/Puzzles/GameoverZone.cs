using UnityEditor;
using UnityEngine;


public class GameoverZone : MonoBehaviour
{
    public bool destroyPickable = true;

    private MenuManager menuManager;

    private void Start()
    {
        menuManager = Object.FindFirstObjectByType<MenuManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //DataPersitanceManager.instance.LoadGame();
            menuManager.LoadGameStateGame();
        }

        if (destroyPickable && other.GetComponent<Interactable>() != null && other.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
        {
            Destroy(other.gameObject);
        }   
    }


    private void FinishGame()
    {
        // TODO: Make a Gameover Menu with an option to restart the level
        menuManager.LoadGameStateMenu();
    }
}
