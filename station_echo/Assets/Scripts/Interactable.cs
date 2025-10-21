using UnityEngine;


public enum InteractionType
{
    Pickable,
    Pressable
};
/// <summary>
/// Used to determine wheather the player can interact with this object (pick it up / press it)
/// </summary>
public class Interactable : MonoBehaviour
{
    public InteractionType interactionType;


    public InteractionType GetInteractionType()
    {
        return interactionType;
    }
}


