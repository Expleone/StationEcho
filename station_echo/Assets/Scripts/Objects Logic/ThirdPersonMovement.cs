using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform mesh;
    public Transform meshToRotate;
    public CameraTargetPoint cameraTargetPoint;

    public float speed = 6f;
    public float rotationSpeed = 5f;
    public float cameraOffsetSpeed = 0.2f;
    public float turnSmoothTime = 0.1f;
    public float jumpForce = 0.5f;
    public float groundCheckDistance = 0.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    private bool isHit;
    private RaycastHit hit;
    private bool doubleJumpUsed = false;
    private Animator animator;
    private MaterialSwapper swapper;
    private bool wasRunning;
    private Vector3 velocity;

    bool IsGrounded()
    {
        Vector3 gravityDirection = Physics.gravity.normalized;
        groundCheckRadius = controller.radius;
        groundCheckDistance = controller.height / 2f - controller.radius + 0.1f;

        Vector3 castOrigin = transform.position;

        isHit = Physics.SphereCast(castOrigin, groundCheckRadius, gravityDirection, out hit, groundCheckDistance, groundMask);
        return isHit;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            if (isHit)
            {
                Gizmos.color = Color.red;
                Vector3 hitSphereCenter = transform.position + Physics.gravity.normalized * hit.distance;
                Gizmos.DrawWireSphere(hitSphereCenter, groundCheckRadius);
                Gizmos.DrawLine(transform.position, hitSphereCenter);
                Gizmos.DrawSphere(hit.point, 0.05f);
            }
            else
            {
                Gizmos.color = Color.green;
                Vector3 endPosition = transform.position + Physics.gravity.normalized * (controller.height / 2f - controller.radius + 0.1f);
                Gizmos.DrawLine(transform.position, endPosition);
                Gizmos.DrawWireSphere(endPosition, groundCheckRadius);
            }
        }
    }

    void Start()
    {
        animator = mesh.GetComponent<Animator>();
        swapper = mesh.GetComponentInChildren<MaterialSwapper>();
        wasRunning = false;

        if (cameraTargetPoint == null)
        {
            Debug.LogWarning("CameraTargetPoint does not exist!");
        }
    }

    void Update()
    {
        Vector2 direction2d = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        Vector3 direction = new Vector3(direction2d.x, 0f, direction2d.y).normalized;

        bool isGrounded = IsGrounded();

        if (isGrounded && Vector3.Dot(velocity, Physics.gravity.normalized) > 0)
        {
            velocity = Physics.gravity.normalized * (Physics.gravity.magnitude / 3f);
            doubleJumpUsed = false;
        }

        if (InputSystem.actions.FindAction("Jump").triggered && isGrounded)
        {
            velocity = -Physics.gravity.normalized * jumpForce;
            animator.SetTrigger("jump");
        }
        else if (InputSystem.actions.FindAction("Jump").triggered && !isGrounded && !doubleJumpUsed)
        {
            velocity = -jumpForce * Physics.gravity.normalized;
            doubleJumpUsed = true;
        }

        if (InputSystem.actions.FindAction("Sprint").IsPressed())
        {
            speed = 10f;
        }
        else
        {
            speed = 6f;
        }

        velocity += Physics.gravity * Time.deltaTime;

        Vector3 up = -Physics.gravity.normalized;

        //Vector3 targetOffset = -Physics.gravity.normalized * cameraTargetPoint.newOffset.magnitude;
        cameraTargetPoint.newOffset = -Physics.gravity.normalized * cameraTargetPoint.newOffset.magnitude;

        Vector3 right = Vector3.Cross(Vector3.forward, up).normalized;
        Vector3 forward = Vector3.Cross(up, right).normalized;

        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.right, up).normalized;
        Vector3 moveDirection = (camRight * direction2d.x + camForward * direction2d.y).normalized;

        Vector3 targetForward;
        if (moveDirection.magnitude >= 0.1f)
        {
            targetForward = moveDirection;
        }
        else
        {
            targetForward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            if (targetForward.sqrMagnitude < 0.01f)
            {
                targetForward = camForward;
            }
        }

        if (targetForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetForward, up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            float inAirCorrection = isGrounded ? 1f : 0.5f;
            controller.Move(moveDirection * (speed * Time.deltaTime * inAirCorrection));
        }

        controller.Move(velocity * Time.deltaTime);

        setAnimation(moveDirection.magnitude);
    }

    private void setAnimation(float magnitude)
    {
        bool isRunning = InputSystem.actions.FindAction("Sprint").IsPressed();

        if (isRunning != wasRunning)
        {
            wasRunning = isRunning;
            if (isRunning)
            {
                swapper.SetMaterial(0, "eyes_run");
            }
            else
            {
                swapper.SetMaterial(0, "eyes_idle");
            }
        }

        if (IsGrounded())
        {
            animator.SetBool("isInAir", false);
        }
        else
        {
            animator.SetBool("isInAir", true);
        }

        float animationSpeed = 0f;
        if (magnitude >= 0.1f)
        {
            animationSpeed = InputSystem.actions.FindAction("Sprint").IsPressed() ? 2f : 1f;
        }
        animator.SetFloat("Speed", animationSpeed);
    }
}