using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTracker : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float baseCameraDistance;

    private InputAction look;
    
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseCameraDistance = 8f;
        look = InputSystem.actions.FindAction("Look");
        offset = new Vector3(0, baseCameraDistance / 2, -baseCameraDistance);
        transform.position = player.position + offset;
        transform.LookAt(player);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookVector = 0.5f * look.ReadValue<Vector2>();
        offset = Quaternion.AngleAxis(lookVector.x, Vector3.up) * offset;
        offset = Quaternion.AngleAxis(-lookVector.y, Vector3.right) * offset;
        transform.position = player.position + offset;
        //cammera collision with objects
        RaycastHit hit;
        if (Physics.Linecast(player.position, transform.position, out hit))
        {
            transform.position = hit.point + (player.position - hit.point).normalized * 1f;
        }
        transform.LookAt(player);
    }
    
}
