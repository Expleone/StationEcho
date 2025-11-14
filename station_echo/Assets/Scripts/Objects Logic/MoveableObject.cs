using NUnit.Framework;
using UnityEngine;


enum ObjectType
{
    Platform,
    Pickable,
    Unpickable,
    Player
}
public class MoveableObject : MonoBehaviour
{
    Vector3 propagationMovement;
    bool propagate=true;
    ObjectType objectType;

    PlatformLogic platformLogic;

    ThirdPersonMovement thirdPersonMovement;

    [SerializeField] LayerMask groundMask;
    RaycastHit groundHit;

    Rigidbody rb;


    public Vector3 GetPropagationMovement()
    {
        if (propagate)
        {
            
            return propagationMovement;  
        } 
        return Vector3.zero;
    }
    bool CheckGround()
    {
        // Get the gravity direction (normalized)
        Vector3 castDirection = Physics.gravity.normalized;
        float groundCheckDistance = transform.localScale.y / 2f + 0.1f;
        Vector3 castOrigin = transform.position;


        // Perform the raycast
        bool isHit = Physics.Raycast(castOrigin, castDirection, out groundHit, groundCheckDistance, groundMask);

        return isHit;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectType = GetObjectType(this.gameObject);
        if (objectType == ObjectType.Platform)
        {
            platformLogic = this.gameObject.GetComponentInParent<PlatformLogic>();
        }
        else if (objectType == ObjectType.Player)
        {
            thirdPersonMovement = this.gameObject.GetComponent<ThirdPersonMovement>();
            propagate = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (objectType == ObjectType.Platform)
        {
            propagationMovement = platformLogic.GetPropagationMovement()/ Time.fixedDeltaTime;
        }

        if (objectType == ObjectType.Player)
        {
            groundHit = thirdPersonMovement.GetGroundHit();
            if (groundHit.transform != null)
            {
                MoveableObject groundMoveableObject = groundHit.transform.gameObject.GetComponent<MoveableObject>();
                if (groundMoveableObject != null)
                {
                    propagationMovement = groundMoveableObject.GetPropagationMovement();
                }
                else
                {
                    propagationMovement = new Vector3(0, 0, 0);
                }
            }
            Debug.Log("Player propagation movement: " + propagationMovement);
            thirdPersonMovement.Move(propagationMovement * Time.deltaTime);
        }

        if (objectType == ObjectType.Pickable)
        {
            if (this.transform.parent != null && this.transform.parent.GetComponent<ThirdPersonMovement>() != null){
                propagationMovement = this.transform.parent.GetComponent<ThirdPersonMovement>().GetMovement();
                propagate = false;
            } else {
                propagate = true;
                if (CheckGround())
                {
                    MoveableObject groundMoveableObject = groundHit.transform.gameObject.GetComponent<MoveableObject>();
                    if (groundMoveableObject != null)
                    {
                        propagationMovement = groundMoveableObject.GetPropagationMovement();
                    }
                    else
                    {
                        propagationMovement = new Vector3(0, 0, 0);
                    }
                }
                
            }
        }

        if (objectType == ObjectType.Unpickable)
        {
            if (CheckGround())
            {
                MoveableObject groundMoveableObject = groundHit.transform.gameObject.GetComponent<MoveableObject>();
                if (groundMoveableObject != null)
                {
                    propagationMovement = groundMoveableObject.GetPropagationMovement();
                }
                else
                {
                    propagationMovement = new Vector3(0, 0, 0);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (objectType != ObjectType.Player && objectType != ObjectType.Platform)
        {
            rb = this.gameObject.GetComponent<Rigidbody>();
            if(propagate) rb.MovePosition(rb.position + propagationMovement * Time.fixedDeltaTime);
        }
    }
    
    ObjectType GetObjectType(GameObject obj)
    {
        if (obj.GetComponent<ThirdPersonMovement>() != null)
        {
            return ObjectType.Player;
        }
        else if (obj.GetComponent<Interactable>() != null && obj.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
        {
            return ObjectType.Pickable;
        }
        else if (obj.GetComponentInParent<PlatformLogic>() != null)
        {
            return ObjectType.Platform;
        }
        else
        {
            return ObjectType.Unpickable;
        }
    }
}