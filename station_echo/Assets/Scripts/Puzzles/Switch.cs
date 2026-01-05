using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour
{
    [Header("Switch Settings")]
    public bool IsOn = false;
    public Transform SwitchVisual;
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

    [SerializeField] private Switch SameAs = null;
    private bool sameVal = false;
    private Interactable interactableComponent;

    public PuzzleMarkController puzzleMarkController = null;

    private void Awake()
    {
        if (SwitchVisual == null)
            SwitchVisual = transform.GetChild(0);

        visualStartPos = SwitchVisual.localPosition;

        pressedPosition = visualStartPos + PressDirection.normalized * PressDistance;

        swapper = SwitchVisual.GetComponent<MaterialSwapper>();
        interactableComponent = GetComponent<Interactable>();

        ApplyState();
    }

    private void Update()
    {
        if(SameAs != null && sameVal != SameAs.IsOn)
        {
            Toggle();
            sameVal = SameAs.IsOn;
        }
        if (interactableComponent.HasBeenInteractedWith())
        {

            Toggle();
            sameVal = IsOn;
            interactableComponent.ResetInteraction();
        }
    }

    private void Toggle()
    {
        if (IsOn)
            Unpress();
        else
            Press();
    }

    private void Press()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsOn = true;
        swapper?.SetMaterial(0, PressMaterial);
        animationCoroutine = StartCoroutine(Move(SwitchVisual.localPosition, pressedPosition));

        if (puzzleMarkController)
        {
            foreach (MaterialSwapper ms in puzzleMarkController.childObjects)
                ms.SetMaterial(0, ACT_MAT_PUZZLE_MARK);
        }
    }

    private void Unpress()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsOn = false;
        swapper?.SetMaterial(0, BaseMaterial);
        animationCoroutine = StartCoroutine(Move(SwitchVisual.localPosition, visualStartPos));

        if (puzzleMarkController)
        {
            foreach (MaterialSwapper ms in puzzleMarkController.childObjects)
                ms.SetMaterial(0, DEF_MAT_PUZZLE_MARK);
        }
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            SwitchVisual.localPosition = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.deltaTime * Speed;
            yield return null;
        }
        SwitchVisual.localPosition = to;
    }

    public IEnumerator ApplyState()
    {
        yield return null;

        if (!IsOn)
            Unpress();
        else
            Press();
    }
}
