using UnityEngine;

public class PrintVelocity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        if(rb != null){
            Debug.Log("Velocity of " + gameObject.name + ": " + rb.linearVelocity);
        }
    }
}
