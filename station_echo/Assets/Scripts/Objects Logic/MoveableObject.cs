using NUnit.Framework;
using UnityEngine;

enum ObjectType
{
    Platform,
    Pickable,
    Unpickable,
    Player
}
[DefaultExecutionOrder(0)]
public class MoveableObject : MonoBehaviour
{
    Vector3 propagationMovement;
    Vector3 selfVelosity;
    bool propagate = true;
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
        Vector3 castDirection = Physics.gravity.normalized;
        float groundCheckDistance = transform.localScale.y / 2f + 0.1f;
        Vector3 castOrigin = transform.position;
        bool isHit = Physics.Raycast(castOrigin, castDirection, out groundHit, groundCheckDistance, groundMask);
        return isHit;
    }

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>(); // Get this once
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

    void FixedUpdate()
    {
        if (objectType == ObjectType.Platform)
        {
            propagationMovement = platformLogic.GetPropagationMovement();
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
                    propagationMovement = Vector3.zero;
                }
            }

            if (propagationMovement != Vector3.zero)
            {
                thirdPersonMovement.Move(propagationMovement);
                // print("CharacterController velocity: " + thirdPersonMovement.GetVelocity());
            }
        }
        else if (objectType == ObjectType.Unpickable || objectType == ObjectType.Pickable)
        {
            if (objectType == ObjectType.Pickable && this.gameObject.GetComponent<Interactable>().IsBeingHeld())
            {
                propagationMovement = Vector3.zero;
                return;
            }
            if (CheckGround())
            {
                MoveableObject groundMoveableObject = groundHit.transform.gameObject.GetComponent<MoveableObject>();
                if (groundMoveableObject != null)
                {
                    propagationMovement = groundMoveableObject.GetPropagationMovement();
                }
                else
                {
                    propagationMovement = Vector3.zero;
                }
            }
        }

        if (objectType != ObjectType.Player && objectType != ObjectType.Platform)
        {
            if (propagate && rb != null && propagationMovement != Vector3.zero)
            {
                rb.Move(rb.position + propagationMovement, rb.rotation);
            }
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