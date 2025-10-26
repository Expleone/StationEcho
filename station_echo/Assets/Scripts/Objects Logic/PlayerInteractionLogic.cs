using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractionLogic : MonoBehaviour
{
    [SerializeField] public LayerMask layerMask;
    [SerializeField] public float distanceToPickableItem;
    public List<GameObject> availableInteractions = new List<GameObject>();
    public List<GameObject> unavailableInteractions = new List<GameObject>();
    private GameObject currentlyHolding = null;

    void Start()
    {
    
    }

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

        if (currentlyHolding)
        {
            CheckCollisionWithWalls();
        }
    }
    
    private void CheckCollisionWithWalls()
    {
        float maxDistance = currentlyHolding.transform.localScale.x + distanceToPickableItem;
        RaycastHit hit;

        if (Physics.BoxCast(transform.position, currentlyHolding.transform.localScale / 2, transform.forward, out hit,
                             Quaternion.identity, maxDistance, layerMask))
        {
            // Debug.Log("###########");
            // Debug.Log("BoxCast hit: " + hit.collider.name);
            // Debug.Log("Hit point: " + hit.point);
            Vector3 forwardCopy = transform.forward;
            forwardCopy *= maxDistance - hit.distance + 0.01f;
            transform.position -= forwardCopy;
            // float y = transform.position.y;
            // transform.position = hit.point - (new Vector3(0, 0, distanceToPickableItem) + currentlyHolding.transform.localScale + transform.localScale);
            // transform.position.Set(transform.position.x, y, transform.position.z);
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