using UnityEngine;

public class UpdateInteractionArea : MonoBehaviour
{
    private PlayerInteractionLogic playerInteractionLogic;
    void Start()
    {
        playerInteractionLogic = GetComponentInParent<PlayerInteractionLogic>();
    }

    void Update()
    {
        checkUnavailableInteractions();
    }

    private void checkUnavailableInteractions()
    {
        if (playerInteractionLogic.unavailableInteractions.Count == 0) return;
        for (int i = playerInteractionLogic.unavailableInteractions.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = playerInteractionLogic.unavailableInteractions[i];
            if (!CheckLineOfSight(gameObject))
            {
                playerInteractionLogic.availableInteractions.Add(gameObject);
                playerInteractionLogic.unavailableInteractions.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Interactable>()) return;
        if (other.gameObject.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
        {
            PickableTriggerEnter(other);
        }
        else if (other.gameObject.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pressable)
        {
            PressableTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<Interactable>()) return;
        if (other.gameObject.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pickable)
        {
            PickableTriggerExit(other);
        }
         else if (other.gameObject.GetComponent<Interactable>().GetInteractionType() == InteractionType.Pressable)
        {
            PressableTriggerExit(other);
        }
    }


    private void PickableTriggerEnter(Collider other)
    {
        if (CheckLineOfSight(other.gameObject))
        {
            playerInteractionLogic.unavailableInteractions.Add(other.gameObject);
        }
        else
        {
            playerInteractionLogic.availableInteractions.Add(other.gameObject);
        }
    }


    private void PickableTriggerExit(Collider other)
    {
        if (!playerInteractionLogic.availableInteractions.Remove(other.gameObject))
        {
            playerInteractionLogic.unavailableInteractions.Remove(other.gameObject);
        }
    }

    private void PressableTriggerEnter(Collider other)
    {
        // To Implement
    }
    

    private void PressableTriggerExit(Collider other)
    {
        // To Implement
    }

    // false - if clear, true - not clear
    private bool CheckLineOfSight(GameObject targetObject) 
    {
        Vector3 startPoint = playerInteractionLogic.transform.position;
        Vector3 endPoint = targetObject.gameObject.transform.position;

        Vector3 direction = (endPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, endPoint);
        RaycastHit hit;
       
        if (Physics.Raycast(startPoint, direction, out hit, distance, playerInteractionLogic.layerMask))
        {
            if (hit.transform == targetObject.gameObject.transform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
           return false;
        }
    }
}
