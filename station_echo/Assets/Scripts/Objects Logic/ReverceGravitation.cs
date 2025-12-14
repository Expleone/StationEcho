using UnityEngine;

public class ReverceGravitation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.useGravity)
        rb.AddForce(-2 * Physics.gravity, ForceMode.Acceleration);
    }
}
