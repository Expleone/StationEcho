using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool IsPressed = false;
    public Transform PlateVisual;
    public float Speed = 5f;
    public Vector3 PressDirection = Vector3.down;
    private float PressAmount;
    private Coroutine AnimationCoroutine;
    private List<GameObject> objectsOnPlate = new List<GameObject>();
    private Vector3 visualStartPos;
    private MaterialSwapper swapper;
    public string PressMaterial = "pressed";
    public string BaseMaterial = "base";
    public string DEF_MAT_PUZZLE_MARK = "DEF";
    public string ACT_MAT_PUZZLE_MARK = "ACT";

    public  PuzzleMarkController puzzleMarkController = null;

    private void Awake()
    {
        if (PlateVisual == null)
            PlateVisual = transform.GetChild(0);
        visualStartPos = PlateVisual.localPosition;
	    swapper = PlateVisual.GetComponent<MaterialSwapper>();
        PressAmount = Mathf.Abs(PlateVisual.localScale.y * 0.9f);
    }

    void Update()
    {
        for (int i = objectsOnPlate.Count -1; i >= 0; i--)
        {
            if(objectsOnPlate[i] == null)
            {
                objectsOnPlate.RemoveAt(i);
            }
        }
        if (IsPressed && objectsOnPlate.Count == 0)
        {
            Unpress();
            swapper.SetMaterial(0, BaseMaterial);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerInteractionArea"))
            return;
        objectsOnPlate.Add(other.gameObject);
        if (!IsPressed && objectsOnPlate.Count > 0)
        {
            Press();
	    swapper.SetMaterial(0, PressMaterial);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerInteractionArea"))
            return;
        objectsOnPlate.Remove(other.gameObject);
        if (IsPressed && objectsOnPlate.Count == 0)
        {
            Unpress();
	    swapper.SetMaterial(0, BaseMaterial);
        }
    }

    private void Press()
    {
        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);
        IsPressed = true;
        AnimationCoroutine = StartCoroutine(Move(visualStartPos, visualStartPos + PressDirection * PressAmount));
        if (puzzleMarkController)
        {
            foreach(MaterialSwapper ms in puzzleMarkController.childObjects)
            {
                ms.SetMaterial(0, ACT_MAT_PUZZLE_MARK);
            }
        }
    }

    private void Unpress()
    {
        if (AnimationCoroutine != null)
            StopCoroutine(AnimationCoroutine);
        IsPressed = false;
        AnimationCoroutine = StartCoroutine(Move(PlateVisual.localPosition, visualStartPos));
        if (puzzleMarkController)
        {
            foreach(MaterialSwapper ms in puzzleMarkController.childObjects)
            {
                ms.SetMaterial(0, DEF_MAT_PUZZLE_MARK);
            }
        }
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