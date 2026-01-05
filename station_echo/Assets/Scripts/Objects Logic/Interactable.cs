using System.Runtime.CompilerServices;
using UnityEngine;



public enum InteractionType
{
    Pickable,
    Pressable
};
/// <summary>
/// Used to determine whether the player can interact with this object (pick it up / press it)
/// </summary>
public class Interactable : MonoBehaviour
{
    public InteractionType interactionType;

    private bool haveBeenInteractedWith = false;

    private Transform bearerTransform;

    public void SetBearerTransform(Transform bearer)
    {
        bearerTransform = bearer;
    }
    public bool IsBeingHeld()
    {
        return bearerTransform != null;
    }
    public Transform GetBearerTransform()
    {
        return bearerTransform;
    }
    public InteractionType GetInteractionType()
    {
        return interactionType;
    }

    public void Interact()
    {
        // Interaction logic would go here
        haveBeenInteractedWith = true;
    }

    public bool HasBeenInteractedWith()
    {
        return haveBeenInteractedWith;
    }

    public void ResetInteraction()
    {
        haveBeenInteractedWith = false;
    }
}


