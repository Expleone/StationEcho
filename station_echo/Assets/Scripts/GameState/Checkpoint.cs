using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool visible = true;

    // [Header("UI")]
    // public TextMeshProUGUI saveMessageText;
    private MeshRenderer meshRenderer;
    private bool triggered = false;

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
        if (other.CompareTag("Player") && !triggered)
        {
            Debug.Log("Game saved");
            DataPersitanceManager.instance.SaveGame();
            triggered = true;
        }
    }
}
