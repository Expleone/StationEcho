using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetPoint : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    public Vector3 newOffset;
    public float smoothTime = 5f;
    private Vector3 offsetVelocity = Vector3.zero;

    private Vector3 previousOffset;
    private GameObject parentObject;
    private float transitionProgress = 1f;
    private bool isTransitioning = false;

    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;

    private float startTop, startCenter, startBottom;
    private float targetTop, targetCenter, targetBottom;

    private void Start()
    {
        newOffset = offset;
        previousOffset = offset;
        parentObject = transform.parent.gameObject;
        orbitalFollow = (CinemachineOrbitalFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    void Update()
    {
        if (!isTransitioning && Vector3.Dot(newOffset, Physics.gravity.normalized) > 0)
        {
            StartTransition();
        }

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / smoothTime;
            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
                isTransitioning = false;
            }

            offset = Vector3.Lerp(previousOffset, newOffset, transitionProgress);

            orbitalFollow.Orbits.Top.Height = Mathf.Lerp(startTop, targetTop, transitionProgress);
            orbitalFollow.Orbits.Center.Height = Mathf.Lerp(startCenter, targetCenter, transitionProgress);
            orbitalFollow.Orbits.Bottom.Height = Mathf.Lerp(startBottom, targetBottom, transitionProgress);
        }

        transform.position = parentObject.transform.position + offset;
    }

    private void StartTransition()
    {
        isTransitioning = true;
        transitionProgress = 0f;

        previousOffset = newOffset;
        newOffset = -offset;

        startTop = orbitalFollow.Orbits.Top.Height;
        startCenter = orbitalFollow.Orbits.Center.Height;
        startBottom = orbitalFollow.Orbits.Bottom.Height;

        targetTop = -startTop;
        targetCenter = -startCenter;
        targetBottom = -startBottom;

        parentObject.transform.Rotate(Vector3.forward, 180f);
    }
}
