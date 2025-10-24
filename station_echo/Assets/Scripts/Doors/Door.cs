using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsLocked = false;
    public bool IsOpen = false;
    public float Speed = 1f;
    private Vector3 SlideDirection = Vector3.left;
    private float SlideAmount = 3f;
    private Vector3 StartPosition;
    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartPosition = transform.position;
        SlideAmount = Mathf.Abs(transform.localScale.x);
    }

    public void Interact()
    {

    }

    public void Open()
    {
        if (!IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            IsOpen = true;
            AnimationCoroutine = StartCoroutine(SlideDoor(StartPosition, StartPosition + SlideDirection * SlideAmount));
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            IsOpen = false;
            AnimationCoroutine = StartCoroutine(SlideDoor(transform.position, StartPosition));
        }
    }

    private IEnumerator SlideDoor(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.deltaTime * Speed;
            yield return null;
        }
        transform.position = to;
    }
}