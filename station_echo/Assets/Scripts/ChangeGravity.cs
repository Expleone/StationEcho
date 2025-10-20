using UnityEngine;

public class ChangeGravity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    CameraTargetPoint cameraTargetPoint;
    Transform playerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")){
            return;
        }

        cameraTargetPoint = other.GetComponentInChildren<CameraTargetPoint>();
        playerTransform = other.transform;

        Physics.gravity = Physics.gravity * -1f;
        cameraTargetPoint.newOffset = -Physics.gravity.normalized * cameraTargetPoint.newOffset.magnitude;

        playerTransform.Rotate(Vector3.forward, 180f);
    }
}
