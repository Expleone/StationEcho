using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject itemPrefab;
    private GameObject spawnedObject;
    private Transform pivot;

    private void Start()
    {
        pivot = transform.Find("Pivot");

        if (pivot == null)
        {
            Debug.LogError("Pivot not found as child of Spawner!");
            return;
        }

        SpawnObject();
    }

    void SpawnObject()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("Item prefab not assigned in Spawner!");
            return;
        }

        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        spawnedObject = Instantiate(itemPrefab, pivot.position, pivot.rotation);
    }

    void Update()
    {
        if (spawnedObject == null)
        {
            SpawnObject();
        }
    }
}

