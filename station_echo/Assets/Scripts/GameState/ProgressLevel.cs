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
    public MenuManager menuManager;
    public int NextLevelId;
    public string NextLevelName;
    public bool MoveToMenu;

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

        if (menuManager == null)
        {
            menuManager = FindFirstObjectByType<MenuManager>();
            if (menuManager == null)
            {
                Debug.LogWarning("MenuManager not found in the scene.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UpdateGameState(GameState.Game);
            DataPersitanceManager.instance.NewGame();
            DataPersitanceManager.instance.SetCurrentLevelAsCompleted();
            if (MoveToMenu)
            {
                menuManager.LoadGameStateMenu();
                return;
            }
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
