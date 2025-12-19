using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressLevel : MonoBehaviour
{
    [SerializeField] private bool visible = true;
    [Header("File Storage Config")]
    [SerializeField]
    private bool useLevelName;
    // [Header("UI")]
    // public TextMeshProUGUI saveMessageText;
    private MeshRenderer meshRenderer;
    public int NextLevelId;
    public string NextLevelName;

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
            if (useLevelName)
            {
                SceneManager.LoadScene(NextLevelName);
            }
            else
            {
                SceneManager.LoadScene("LVL" + NextLevelId);
            }
            Physics.gravity = new Vector3(0, -9.8f, 0);
        }
    }
}
