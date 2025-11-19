using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
    [Header("Button Settings")]
    public bool IsPressed = false;
    public float ActiveTime = 3f;
    public Transform ButtonVisual;
    public float Speed = 5f;
    public Vector3 PressDirection = Vector3.down;
    public string PressMaterial = "pressed";
    public string BaseMaterial = "base";

    [Header("Input Settings")]
    public InputActionAsset inputActions;
    public string actionMapName = "Player";
    public string interactActionName = "Interact";
    private Vector3 visualStartPos;
    private Coroutine animationCoroutine;
    private MaterialSwapper swapper;
    private bool playerInRange = false;
    private InputAction interactAction;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        if (ButtonVisual == null)
            ButtonVisual = transform.GetChild(0);

        visualStartPos = ButtonVisual.localPosition;
        swapper = ButtonVisual.GetComponent<MaterialSwapper>();
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            var map = inputActions.FindActionMap(actionMapName);
            if (map != null)
            {
                interactAction = map.FindAction(interactActionName);
                interactAction?.Enable();
            }
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
            if (!IsPressed)
                Press();
        }
    }

    private void Press()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        IsPressed = true;
        swapper?.SetMaterial(0, PressMaterial);
        animationCoroutine = StartCoroutine(Move(ButtonVisual.localPosition, visualStartPos + PressDirection));

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
