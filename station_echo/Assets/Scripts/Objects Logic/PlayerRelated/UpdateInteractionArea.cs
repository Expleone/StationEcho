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
        checkAvailableInteractions();
    }

    private void checkUnavailableInteractions()
    {
        if (playerInteractionLogic.unavailableInteractions.Count == 0) return;
        for (int i = playerInteractionLogic.unavailableInteractions.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = playerInteractionLogic.unavailableInteractions[i];
            if(gameObject == null)
            {
                playerInteractionLogic.unavailableInteractions.RemoveAt(i);
                continue;
            }

            if (gameObject.GetComponent<Interactable>().interactionType == InteractionType.Pressable)
            {
                if (!CheckLineOfSight(gameObject) && !gameObject.GetComponent<Interactable>().HasBeenInteractedWith())
                {
                    playerInteractionLogic.availableInteractions.Add(gameObject);
                    playerInteractionLogic.unavailableInteractions.RemoveAt(i);
                }
            }
            else
            {
                if (!CheckLineOfSight(gameObject))
                {
                    playerInteractionLogic.availableInteractions.Add(gameObject);
                    playerInteractionLogic.unavailableInteractions.RemoveAt(i);
                }
            }
        }
    }

    private void checkAvailableInteractions()
    {
        if (playerInteractionLogic.availableInteractions.Count == 0) return;
        for (int i = playerInteractionLogic.availableInteractions.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = playerInteractionLogic.availableInteractions[i];

            if(gameObject == null)
            {
                playerInteractionLogic.availableInteractions.RemoveAt(i);
                continue;
            }

            if (gameObject.GetComponent<Interactable>().interactionType == InteractionType.Pressable)
            {
                if (CheckLineOfSight(gameObject) && gameObject.GetComponent<Interactable>().HasBeenInteractedWith())
                {
                    playerInteractionLogic.unavailableInteractions.Add(gameObject);
                    playerInteractionLogic.availableInteractions.RemoveAt(i);
                }
            }
            else
            {
                if (CheckLineOfSight(gameObject))
                {
                    playerInteractionLogic.unavailableInteractions.Add(gameObject);
                    playerInteractionLogic.availableInteractions.RemoveAt(i);
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // print("Trigger entered: " + other.gameObject.name);
        if (!other.gameObject.GetComponent<Interactable>()) return;

        if (CheckLineOfSight(other.gameObject)
        )
        {
            playerInteractionLogic.unavailableInteractions.Add(other.gameObject);
        }
        else
        {
            playerInteractionLogic.availableInteractions.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<Interactable>()) return;

        if (!playerInteractionLogic.availableInteractions.Remove(other.gameObject))
        {
            playerInteractionLogic.unavailableInteractions.Remove(other.gameObject);
        }
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
