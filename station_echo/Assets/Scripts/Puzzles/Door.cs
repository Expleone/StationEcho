using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsLocked = false;
    public bool IsOpen = false;
    public float Speed = 1f;
    public bool UseTimeBasedMovement = false;
    public float OpenTime = 1f;
    public float SlideDistance = 2f;
    public Vector3 SlideDirection = Vector3.left;

    private Vector3 StartPosition;
    private Vector3 TargetPosition;
    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartPosition = transform.localPosition;
        TargetPosition = StartPosition + SlideDirection.normalized * SlideDistance;
    }

    public void Open()
    {
        if (IsLocked || IsOpen) return;

        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);

        IsOpen = true;
        AnimationCoroutine = StartCoroutine(SlideDoor(transform.localPosition, TargetPosition));
    }

    public void Close()
    {
        if (!IsOpen) return;

        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);

        IsOpen = false;
        AnimationCoroutine = StartCoroutine(SlideDoor(transform.localPosition, StartPosition));
    }

    private IEnumerator SlideDoor(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration;
        if (UseTimeBasedMovement)
        {
            duration = OpenTime;
        }
        else
        {
            duration = Vector3.Distance(from, to) / Speed;
        }

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = to;
    }
}
