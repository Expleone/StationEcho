using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool visible = true;
    private MeshRenderer meshRenderer;
    private bool triggered = false;

#if UNITY_EDITOR
    void OnValidate()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visible;
        }
    }
#endif

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visible;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            DataPersitanceManager.instance.SaveGame();
            triggered = true;
        }
    }

}
