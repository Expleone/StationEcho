using UnityEngine;

public class gravTriggerScript : MonoBehaviour
{
    public float maxFallSpeed = 6;
    public bool first = false;


    void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.GetComponent<ThirdPersonMovement>()) return;
        
        if (first)
        {
            other.gameObject.GetComponent<ThirdPersonMovement>().maxFallSpeed = maxFallSpeed;
        }
        else
        {
            other.gameObject.GetComponent<ThirdPersonMovement>().maxFallSpeed = float.MaxValue;
        }
        Destroy(gameObject);
    }
}
