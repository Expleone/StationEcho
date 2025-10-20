using UnityEngine;

public class CameraTargetPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    public Vector3 newOffset;
    public float smoothTime = 0.01f;
    private Vector3 offsetVelocity = Vector3.zero;

    private void Start()
    {
        // Initialize the target point for the camera
        newOffset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly move the offset to the new offset position using SmoothDamp
        offset = Vector3.SmoothDamp(offset, newOffset, ref offsetVelocity, smoothTime);

        transform.position = transform.parent.position + offset;
    }
}
