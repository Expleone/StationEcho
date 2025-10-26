using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool IsPressed = false;
    public Transform PlateVisual;
    public float Speed = 5f;
    public Vector3 PressDirection = Vector3.down;
    private float PressAmount;
    private Coroutine AnimationCoroutine;
    private int objectsOnPlate = 0;
    private Vector3 visualStartPos;
    private MaterialSwapper swapper;

    private void Awake()
    {
        if (PlateVisual == null)
            PlateVisual = transform.GetChild(0);
        visualStartPos = PlateVisual.localPosition;
	swapper = PlateVisual.GetComponent<MaterialSwapper>();
        PressAmount = Mathf.Abs(PlateVisual.localScale.y * 0.9f);
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsOnPlate++;
        if (!IsPressed && objectsOnPlate > 0)
        {
            Press();
	    swapper.SetMaterial(0, "pressed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
        if (IsPressed && objectsOnPlate == 0)
        {
            Unpress();
	    swapper.SetMaterial(0, "base");
        }
    }

    private void Press()
    {
        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);
        IsPressed = true;
        AnimationCoroutine = StartCoroutine(Move(visualStartPos, visualStartPos + PressDirection * PressAmount));
    }

    private void Unpress()
    {
        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);
        IsPressed = false;
        AnimationCoroutine = StartCoroutine(Move(PlateVisual.localPosition, visualStartPos));
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            PlateVisual.localPosition = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.deltaTime * Speed;
            yield return null;
        }
        PlateVisual.localPosition = to;
    }
}