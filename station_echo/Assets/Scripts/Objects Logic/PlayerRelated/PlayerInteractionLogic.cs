using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractionLogic : MonoBehaviour
{
    [SerializeField] public LayerMask layerMask;
    [SerializeField] public float distanceToPickableItem;
    public Transform holdPoint; 
    public float moveForce = 10f;
    public List<GameObject> availableInteractions = new List<GameObject>();
    public List<GameObject> unavailableInteractions = new List<GameObject>();
    private Rigidbody heldRb = null;

    void Start()
    {
    
    }

    void Update()
    {
        if (InputSystem.actions.FindAction("Interact").triggered && !heldRb && availableInteractions.Count != 0)
        {
            availableInteractions.Sort(new SortByProximity(transform));
            heldRb = availableInteractions[0].GetComponent<Rigidbody>();

            //heldRb.useGravity = false;

            heldRb.transform.SetParent(transform);
            
            heldRb.isKinematic = true;
            //otherRigidbody.useGravity = false;
            
            heldRb.transform.localPosition = holdPoint.localPosition;
            heldRb.transform.localRotation = new UnityEngine.Quaternion(0, 0, 0, 0);
        }

        else if (InputSystem.actions.FindAction("Interact").triggered && heldRb)
        {
            heldRb.transform.SetParent(null);
            heldRb.isKinematic = false;
            //heldRb.useGravity = true;

            heldRb = null;
        }   
    }


    void FixedUpdate()
    {
        if (heldRb)
        {
            //MoveObjectToHand();
            //CheckCollisionWithWalls();
        }
    }


    void MoveObjectToHand()
    {
       // 1. Get the distance to the hand
        Vector3 direction = holdPoint.position - heldRb.position;
        float distance = direction.magnitude;

        // 2. Move it
        // Logic: If far away, move fast. If close, slow down.
        // The 'heldRb.drag' we set earlier prevents this from exploding.
        heldRb.linearVelocity = direction * moveForce;
    }
    
    private void CheckCollisionWithWalls()
    {
        float maxDistance = heldRb.transform.localScale.x + distanceToPickableItem;
        RaycastHit hit;

        if (Physics.BoxCast(transform.position, heldRb.transform.localScale / 2, transform.forward, out hit,
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