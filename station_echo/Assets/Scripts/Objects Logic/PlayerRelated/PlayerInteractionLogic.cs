using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)]
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
    public OutlineAdder outlineAdder;
    
    private GameObject currentPlayerInteraction = null;
    private bool IsDroped  = false;


    private Material[] originalMaterials;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        outlineAdder = GetComponent<OutlineAdder>();
    }

    void Update()
    {
        if (heldRb)
        {
            if (currentPlayerInteraction != null)
            {
                outlineAdder.RemoveOutline(currentPlayerInteraction.transform);
                currentPlayerInteraction = null;
            }
            
            if (InputSystem.actions.FindAction("Interact").triggered)
            {
                IsDroped = true;
            }
            return; 
        }

        if (availableInteractions.Count == 0)
        {
            if (currentPlayerInteraction != null)
            {
                outlineAdder.RemoveOutline(currentPlayerInteraction.transform);
                currentPlayerInteraction = null;
            }
            return;
        }

        availableInteractions.Sort(new SortByProximity(transform));
        GameObject nearestObject = availableInteractions[0];

        if (nearestObject != currentPlayerInteraction)
        {
            if (currentPlayerInteraction != null)
            {
                outlineAdder.RemoveOutline(currentPlayerInteraction.transform);
            }

            outlineAdder.ApplyOutline(nearestObject.transform);
            
            currentPlayerInteraction = nearestObject;
        }

        if (InputSystem.actions.FindAction("Interact").triggered && currentPlayerInteraction != null)
        {
            if (currentPlayerInteraction.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
            {
                heldRb = currentPlayerInteraction.GetComponent<Rigidbody>();
                heldRb.GetComponent<Interactable>().SetBearerTransform(transform);
                heldRb.transform.SetParent(null);
                heldRb.useGravity = false;
                heldRb.rotation = Quaternion.identity;
                heldRb.angularVelocity = Vector3.zero;
                outlineAdder.RemoveOutline(currentPlayerInteraction.transform);
                currentPlayerInteraction = null; 
            }
            else
            {
                currentPlayerInteraction.GetComponent<Interactable>().Interact();
            }
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
        if (IsDroped)
        {
            DropObject();
            IsDroped = false;
            return;
        }

        if(Physics.gravity == new Vector3(0, 0, 1))
        {
            float bottomY = transform.position.y - transform.localScale.y / 2;
            float upperY = heldRb.transform.localScale.y / 2 + heldRb.transform.position.y;

            if(upperY + 0.1f < bottomY)
            {
                DropObject();
            }
        }
        else if (Physics.gravity == new Vector3(0, 0, -1))
        {
            float bottomY = heldRb.transform.localScale.y / 2 + heldRb.transform.position.y;
            float upperY = transform.position.y - transform.localScale.y / 2;

            if(upperY + 0.1f < bottomY)
            {
                DropObject();
            }
        }
    }

    void DropObject()
    {
        heldRb.GetComponent<Interactable>().SetBearerTransform(null);
        heldRb.transform.SetParent(null);
        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.AddForce(characterController.velocity, ForceMode.VelocityChange);
        // print ("Dropped with velocity: " + characterController.velocity);
        heldRb = null;
    }

    



    void MoveObjectToHand()
    {
        Vector3 directionToHand = holdPoint.position - heldRb.position;

        if(directionToHand.magnitude > maxDistanceToObject)
        {
            heldRb.transform.SetParent(null);
            heldRb.useGravity = true;
            heldRb = null;
            return;
        }
        
        
        Vector3 targetVelocity = directionToHand / Time.fixedDeltaTime;

        float maxSpeed = 15f; 
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, maxSpeed);

        if (characterController != null)
        {
            targetVelocity += characterController.velocity;
        }

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