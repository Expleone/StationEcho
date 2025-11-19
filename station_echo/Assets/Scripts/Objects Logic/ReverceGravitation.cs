using UnityEngine;

public class ReverceGravitation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
    }
}
