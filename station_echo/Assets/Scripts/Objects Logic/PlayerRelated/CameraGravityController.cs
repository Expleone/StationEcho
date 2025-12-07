using UnityEngine;

public class CameraGravityController : MonoBehaviour
{
    // Adjust this to make the camera rotation faster/slower
    [SerializeField] private float rotationSpeed = 5f;

    // Default up is World Up
    private Vector3 targetUpVector = Vector3.up;

    void Update()
    {
        // Smoothly rotate this "Reference Object" to match the target Up
        if (transform.up != targetUpVector)
        {
            // Slerp allows for a smooth transition instead of a snappy one
            Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, targetUpVector);
            
            // We use RotateTowards to ensure consistent speed
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRot, 
                rotationSpeed * Time.deltaTime * 10f
            );
        }
    }

    // Call this method whenever you change gravity in your game
    public void SetGravityDirection(Vector3 newGravityDirection)
    {
        // If gravity is Down (-Y), then "Up" is Up (+Y).
        // So Up is always the opposite of Gravity.
        targetUpVector = -newGravityDirection.normalized;
    }
}