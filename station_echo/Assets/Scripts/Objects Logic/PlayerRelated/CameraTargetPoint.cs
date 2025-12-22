using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    public Vector3 newOffset;
    public float smoothTime = 5f;
    private Vector3 offsetVelocity = Vector3.zero;

    public int currentGravityDirection = 1;
    private Vector3 previousOffset = Vector3.zero;
    private GameObject parentObject;
    private float timeSinceChange = 0f;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform playerMesh;
    private CinemachineOrbitalFollow orbitalFollow;
    private float currentDutch;

    CinemachineRecomposer recomposer;
    private void Start()
    {
        currentDutch = 0f;
        // Initialize the target point for the camera
        newOffset = offset;
        cinemachineCamera.TryGetComponent<CinemachineRecomposer>(out recomposer);

        parentObject = transform.parent.gameObject;
        orbitalFollow = (CinemachineOrbitalFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    void Update()
    {
        if (recomposer != null)
        {
            recomposer.Dutch = Mathf.MoveTowards(recomposer.Dutch, currentDutch, 360f * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (Vector3.Dot(newOffset, Physics.gravity.normalized) > 0)
        {
            
            previousOffset = newOffset;
            newOffset = -previousOffset;
            
            orbitalFollow.Orbits.Top.Height = -orbitalFollow.Orbits.Top.Height;
            orbitalFollow.Orbits.Center.Height = -orbitalFollow.Orbits.Center.Height;
            orbitalFollow.Orbits.Bottom.Height = -orbitalFollow.Orbits.Bottom.Height;
        
            if (currentDutch == 180f) currentDutch = 0f;
            else currentDutch = 180f;

            timeSinceChange = 0f;

            playerMesh.Rotate(Vector3.forward, 180f);
            ThirdPersonMovement movement = parentObject.GetComponent<ThirdPersonMovement>();
            if (movement != null && movement.GetGroundHit().collider != null)
            {
                movement.SetIsDoubleJumpUsed(false);
            }

            CinemachineInputAxisController inputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();
            if (inputAxisController != null)
            {
                foreach (var controller in inputAxisController.Controllers)
                {
                    if (controller.Name == "Look Orbit X")
                    {
                        controller.Input.Gain = -controller.Input.Gain;
                    }
                }
            }
            currentGravityDirection *= -1;
            
        }

        timeSinceChange += Time.deltaTime;
        
        offset = Vector3.Lerp(previousOffset, newOffset, timeSinceChange / smoothTime);
        transform.position = parentObject.transform.position + offset;
    }
}
