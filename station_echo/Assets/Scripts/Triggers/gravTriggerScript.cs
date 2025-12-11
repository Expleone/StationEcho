using UnityEngine;

public class gravTriggerScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Physics.gravity = new Vector3(0f, -1f, 0f);
    }
}
