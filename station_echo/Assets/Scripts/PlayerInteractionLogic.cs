using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractionLogic : MonoBehaviour
{
    [SerializeField] public LayerMask layerMask;
    public List<GameObject> availableInteractions = new List<GameObject>();
    public List<GameObject> unavailableInteractions = new List<GameObject>();
    private GameObject currentlyHolding = null;

    void Update()
    {
        if (InputSystem.actions.FindAction("Interact").triggered && !currentlyHolding && availableInteractions.Count != 0)
        {
            availableInteractions.Sort(new SortByProximity(transform));
            currentlyHolding = availableInteractions[0];

            currentlyHolding.transform.SetParent(transform);
            Rigidbody otherRigidbody = currentlyHolding.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.isKinematic = true;
            }
            currentlyHolding.transform.localPosition = new Vector3(0, 0, transform.localScale.z + 0.1f);
            currentlyHolding.transform.localRotation = new UnityEngine.Quaternion(0, 0, 0, 0);
        }

        else if (InputSystem.actions.FindAction("Interact").triggered && currentlyHolding)
        {
            currentlyHolding.transform.SetParent(null);
            Rigidbody otherRigidbody = currentlyHolding.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.isKinematic = false;
            }

            currentlyHolding = null;
        }
    }
}

public class SortByProximity : IComparer<GameObject>
{
    private Transform playerTransform;
    public SortByProximity(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    public int Compare(GameObject x, GameObject y)
    {
        float proximityX = Vector3.Distance(x.transform.position, playerTransform.position);
        float proximityY = Vector3.Distance(y.transform.position, playerTransform.position);
        return proximityX.CompareTo(proximityY);
    }
}