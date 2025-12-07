using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    public bool IsPressed = false;
    public float ActiveTime = 3f;
    public Transform ButtonVisual;
    public float Speed = 5f;

    [Header("Press Offset Settings")]
    public Vector3 PressDirection = Vector3.down;
    public float PressDistance = 0.1f;

    public string PressMaterial = "pressed";
    public string BaseMaterial = "base";
    public string DEF_MAT_PUZZLE_MARK = "DEF";
    public string ACT_MAT_PUZZLE_MARK = "ACT";

    [Header("Input Settings")]
    public InputActionAsset inputActions;
    public string actionMapName = "Player";
    public string interactActionName = "Interact";

    private Vector3 visualStartPos;
    private Vector3 pressedPosition;

    private Coroutine animationCoroutine;
    private MaterialSwapper swapper;
    private InputAction interactAction;
    private Coroutine timerCoroutine;

    private Interactable interactableComponent;

    public PuzzleMarkController puzzleMarkController = null;

    private bool isResetting = false;

    private void Awake()
    {
        if (ButtonVisual == null)
            ButtonVisual = transform.GetChild(0);

        visualStartPos = ButtonVisual.localPosition;

        pressedPosition = visualStartPos + PressDirection.normalized * PressDistance;

        swapper = ButtonVisual.GetComponent<MaterialSwapper>();
        interactableComponent = GetComponent<Interactable>();
    }

    private void OnDisable()
    {
        interactAction?.Disable();
    }

    private void Update()
    {
        if (interactableComponent.HasBeenInteractedWith() && !isResetting)
        {
            Press();
            isResetting = true;
        }
    }

    private void Press()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsPressed = true;
        swapper?.SetMaterial(0, PressMaterial);
        animationCoroutine = StartCoroutine(Move(ButtonVisual.localPosition, pressedPosition));

        if (puzzleMarkController)
        {
            foreach (MaterialSwapper ms in puzzleMarkController.childObjects)
                ms.SetMaterial(0, ACT_MAT_PUZZLE_MARK);
        }

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(ActiveTime);
        Unpress();
    }

    private void Unpress()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsPressed = false;
        swapper?.SetMaterial(0, BaseMaterial);
        animationCoroutine = StartCoroutine(Move(ButtonVisual.localPosition, visualStartPos));

        if (puzzleMarkController)
        {
            foreach (MaterialSwapper ms in puzzleMarkController.childObjects)
                ms.SetMaterial(0, DEF_MAT_PUZZLE_MARK);
            }

        interactableComponent.ResetInteraction();
        isResetting = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            ButtonVisual.localPosition = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.deltaTime * Speed;
            yield return null;
        }
        ButtonVisual.localPosition = to;
    }
}
