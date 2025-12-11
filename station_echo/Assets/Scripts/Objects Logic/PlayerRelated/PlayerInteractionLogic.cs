using System.Collections.Generic;
using System.Linq;
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
    public Material outlineMaterial;
    private GameObject currentPlayerInteraction = null;
    private bool heldGravityMode = false;

    private Material[] originalMaterials;
    void Start()
    {
    
    }

    void Update()
{
    // Drop
    if (heldRb)
    {
        if (currentPlayerInteraction != null)
        {
            RemoveOutline(currentPlayerInteraction.transform);
            currentPlayerInteraction = null;
        }
       
        if (InputSystem.actions.FindAction("Interact").triggered)
        {
            heldRb.transform.SetParent(null);
            heldRb.useGravity = heldGravityMode;
            heldRb.linearVelocity = Vector3.zero;
            heldRb.angularVelocity = Vector3.zero;
            heldRb = null;
        }
        return; 
    }
    // Do nothing (remove outlines)
    if (availableInteractions.Count == 0)
    {
        if (currentPlayerInteraction != null)
        {
            RemoveOutline(currentPlayerInteraction.transform);
            currentPlayerInteraction = null;
        }
        return;
    }
    // Do nothing (draw outlines)
    availableInteractions.Sort(new SortByProximity(transform));
    GameObject nearestObject = availableInteractions[0];

    if (nearestObject != currentPlayerInteraction)
    {
        if (currentPlayerInteraction != null)
        {
            RemoveOutline(currentPlayerInteraction.transform);
        }

        ApplyOutline(nearestObject.transform);
        
        currentPlayerInteraction = nearestObject;
    }
    // Pick Up
    if (InputSystem.actions.FindAction("Interact").triggered && currentPlayerInteraction != null)
    {
        if (currentPlayerInteraction.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
        {
            heldRb = currentPlayerInteraction.GetComponent<Rigidbody>();
            heldGravityMode = heldRb.useGravity;
            heldRb.transform.SetParent(null);
            heldRb.useGravity = false;
            heldRb.rotation = Quaternion.identity;
            
            RemoveOutline(currentPlayerInteraction.transform);
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
        if(Physics.gravity == new Vector3(0, 0, 1))
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
        else if (Physics.gravity == new Vector3(0, 0, -1))
        {
            float bottomY = heldRb.transform.localScale.y / 2 + heldRb.transform.position.y;
            float upperY = transform.position.y - transform.localScale.y / 2;

            if(upperY < bottomY)
            {
                heldRb.transform.SetParent(null);
                heldRb.useGravity = true;
                heldRb.linearVelocity = Vector3.zero;
                heldRb = null;
            }
        }
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


    void ApplyOutline(Transform target)
    {
        Renderer render = target.GetComponent<Renderer>();

        if(render == null) render = target.GetComponentInChildren<Renderer>();
        
        if (render != null)
        {
            List<Material> materials = render.materials.ToList();

            if (materials.Count > 0 && materials[materials.Count - 1].name.StartsWith(outlineMaterial.name))
            {
                return; 
            }

            materials.Add(outlineMaterial);
            render.materials = materials.ToArray();
        }
    }

    void RemoveOutline(Transform target)
    {
        Renderer render = target.GetComponent<Renderer>();
        if(render == null) render = target.GetComponentInChildren<Renderer>();

        if (render != null)
        {
            List<Material> materials = render.materials.ToList();

            if (materials.Count > 1 && materials[materials.Count - 1].name.StartsWith(outlineMaterial.name)) 
            {
                materials.RemoveAt(materials.Count - 1);
                render.materials = materials.ToArray();
            }
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