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
            DataPersitanceManager.instance.SetCurrentLevelAsCompleted();
            SceneManager.LoadScene("LVL" + (currenLevelId + 1));
        }
    }
}
