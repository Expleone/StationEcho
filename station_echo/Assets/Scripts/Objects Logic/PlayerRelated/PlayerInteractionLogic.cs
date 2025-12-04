using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractionLogic : MonoBehaviour
{
    [SerializeField] public LayerMask layerMask;
    [SerializeField] public float distanceToPickableItem;
    public CharacterController characterController;
    public Transform holdPoint; 
    public float moveForce = 40f;
    public float maxDistanceToObject = 5f;
    public List<GameObject> availableInteractions = new List<GameObject>();
    public List<GameObject> unavailableInteractions = new List<GameObject>();
    private Rigidbody heldRb = null;
    private bool heldGravityMode = false;

    void Start()
    {
    
    }

    void Update()
    {
        if (InputSystem.actions.FindAction("Interact").triggered && !heldRb && availableInteractions.Count != 0)
        {
            availableInteractions.Sort(new SortByProximity(transform));
            heldRb = availableInteractions[0].GetComponent<Rigidbody>();
            heldGravityMode = heldRb.useGravity;
            heldRb.transform.SetParent(null);
            heldRb.useGravity = false;
            heldRb.rotation = Quaternion.identity;
        }

        else if (InputSystem.actions.FindAction("Interact").triggered && heldRb)
        {
            heldRb.transform.SetParent(null);
            heldRb.useGravity = heldGravityMode;
            heldRb.linearVelocity = Vector3.zero;
            heldRb = null;
        }   
    }


    void FixedUpdate()
    {
        if (heldRb)
        {
            heldRb.transform.localRotation = transform.rotation;
            MoveObjectToHand();
            if(!heldRb)  return;
            DropLogic();
        }
    }


    void DropLogic()
    {
        float bottomY = transform.position.y - transform.localScale.y / 2;
        float upperY = heldRb.transform.localScale.y / 2 + heldRb.transform.position.y;

        if(upperY < bottomY)
        {
            heldRb.transform.SetParent(null);
            heldRb.useGravity = true;
            heldRb.linearVelocity = Vector3.zero;
            heldRb = null;
        }
    }


    void MoveObjectToHand()
    {
       // 1. Get vector to hand
        Vector3 directionToHand = holdPoint.position - heldRb.position;

        if(directionToHand.magnitude > maxDistanceToObject)
        {
            heldRb.transform.SetParent(null);
            heldRb.useGravity = true;
            heldRb = null;
            return;
        }
        
        // 2. Calculate the velocity needed to reach the hand in ONE physics step
        // This is smoother than raw force because it adjusts automatically based on framerate
        Vector3 targetVelocity = directionToHand / Time.fixedDeltaTime;

        // 3. Clamp the force so it doesn't explode if you turn too fast
        // "15" is the max speed. Lower = smoother/slower. Higher = snappier.
        float maxSpeed = 15f; 
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, maxSpeed);

        // 4. Feed Forward (Your player movement)
        if (characterController != null)
        {
            targetVelocity += characterController.velocity;
        }

        // 5. Apply
        // We use velocity directly instead of force for smoothness
        heldRb.linearVelocity = Vector3.Lerp(heldRb.linearVelocity, targetVelocity, 0.2f);
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