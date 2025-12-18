using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressLevel : MonoBehaviour
{
    [SerializeField] private bool visible = true;

    // [Header("UI")]
    // public TextMeshProUGUI saveMessageText;
    private MeshRenderer meshRenderer;
    public int currenLevelId;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) return;
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = visible;
    }
#endif

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = visible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UpdateGameState(GameState.Game);
            DataPersitanceManager.instance.NewGame();
            DataPersitanceManager.instance.SetCurrentLevelAsCompleted();
            SceneManager.LoadScene("LVL" + (currenLevelId + 1));
        }
    }
}
