using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool visible = true;

    public bool triggerOnlyWithCube = false;

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
            if (!triggerOnlyWithCube)
            {
                Save();
            }
            else
            {
                var interaction = other.GetComponentInParent<PlayerInteractionLogic>();
                bool isHolding = interaction != null && interaction.GetIsHolding();
                if (isHolding)
                {
                    Save();
                }
            }
        }
    }

    void Save()
    {
        Debug.Log("Game saved");
        DataPersitanceManager.instance.SaveGame();
        triggered = true;
    }
}
