using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressLevel : MonoBehaviour
{
    public int currenLevelId;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UpdateGameState(GameState.Game);
            DataPersitanceManager.instance.SaveGame();
            SceneManager.LoadScene("LVL" + (currenLevelId + 1));

        }
    }
}
