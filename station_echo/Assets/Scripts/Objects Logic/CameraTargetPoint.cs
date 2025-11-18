using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    public Vector3 newOffset;
    public float smoothTime = 5f;
    private Vector3 offsetVelocity = Vector3.zero;

    private Vector3 previousOffset = Vector3.zero;
    private GameObject parentObject;
    private float timeSinceChange = 0f;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform playerMesh;
    private CinemachineOrbitalFollow orbitalFollow;
    private void Start()
    {
        // Initialize the target point for the camera
        newOffset = offset;

        parentObject = transform.parent.gameObject;
        orbitalFollow = (CinemachineOrbitalFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Dot(newOffset, Physics.gravity.normalized) > 0)
        {
            
            previousOffset = newOffset;
            newOffset = -previousOffset;
            
            orbitalFollow.Orbits.Top.Height = -orbitalFollow.Orbits.Top.Height;
            orbitalFollow.Orbits.Center.Height = -orbitalFollow.Orbits.Center.Height;
            orbitalFollow.Orbits.Bottom.Height = -orbitalFollow.Orbits.Bottom.Height;
            timeSinceChange = 0f;

            playerMesh.Rotate(Vector3.forward, 180f);
            ThirdPersonMovement movement = parentObject.GetComponent<ThirdPersonMovement>();
            if (movement != null && movement.GetGroundHit().collider != null)
            {
                movement.SetIsDoubleJumpUsed(false);
            }
        }

        timeSinceChange += Time.deltaTime;
        
        offset = Vector3.Lerp(previousOffset, newOffset, timeSinceChange / smoothTime);
        transform.position = parentObject.transform.position + offset;
    }
}
