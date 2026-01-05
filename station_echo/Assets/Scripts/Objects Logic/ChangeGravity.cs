using UnityEngine;
using Unity.Cinemachine;

public class ChangeGravity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] public CameraTargetPoint cameraTargetPoint;
    Transform playerTransform;


    

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // CinemachineCamera cinemachineCamera = other.GetComponentInChildren<CinemachineCamera>();
        // CinemachineFreeLookModifier freeLookModifier = cinemachineCamera.GetComponent<CinemachineFreeLookModifier>();

        

        playerTransform = other.transform;

        Physics.gravity = Physics.gravity * -1f;

        // playerTransform.Rotate(Vector3.forward, 180f);
    }
}
