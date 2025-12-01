using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : MonoBehaviour
{
    [Header("Switch Settings")]
    public bool IsOn = false;
    public Transform SwitchVisual;
    public float Speed = 5f;
    public Vector3 PressDirection = Vector3.down;
    public string PressMaterial = "pressed";
    public string BaseMaterial = "base";
    public string DEF_MAT_PUZZLE_MARK = "DEF";
    public string ACT_MAT_PUZZLE_MARK = "ACT";

    [Header("Input Settings")]
    public InputActionAsset inputActions;
    public string actionMapName = "Player";
    public string interactActionName = "Interact";

    private Vector3 visualStartPos;
    private Coroutine animationCoroutine;
    private MaterialSwapper swapper;
    private bool playerInRange = false;
    private InputAction interactAction;

    public  PuzzleMarkController puzzleMarkController = null;

    private void Awake()
    {
        if (SwitchVisual == null)
            SwitchVisual = transform.GetChild(0);

        visualStartPos = SwitchVisual.localPosition;
        swapper = SwitchVisual.GetComponent<MaterialSwapper>();
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            var map = inputActions.FindActionMap(actionMapName);
            if (map != null)
            {
                interactAction = map.FindAction(interactActionName);
                if (interactAction != null)
                    interactAction.Enable();
                else
                    Debug.LogWarning($"Action '{interactActionName}' not found in ActionMap '{actionMapName}'.");
            }
            else
            {
                Debug.LogWarning($"ActionMap '{actionMapName}' not found in InputActionAsset '{inputActions.name}'.");
            }
        }
        else
        {
            Debug.LogWarning("InputActionAsset not assigned to Switch!");
        }
    }

    private void OnDisable()
    {
        interactAction?.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Update()
    {
        if (playerInRange && interactAction != null && interactAction.triggered)
        {
            Toggle();
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
        animationCoroutine = StartCoroutine(Move(SwitchVisual.localPosition, visualStartPos + PressDirection));

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
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsOn = false;
        swapper?.SetMaterial(0, BaseMaterial);
        animationCoroutine = StartCoroutine(Move(SwitchVisual.localPosition, visualStartPos));

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
            SwitchVisual.localPosition = Vector3.Lerp(from, to, elapsed);
            elapsed += Time.deltaTime * Speed;
            yield return null;
        }
        SwitchVisual.localPosition = to;
    }
}
