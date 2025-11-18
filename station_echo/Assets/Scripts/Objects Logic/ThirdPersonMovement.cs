using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CharacterController controller;
    public Transform cam;
    public Transform mesh;
    public Transform meshToRotate;
    public CameraTargetPoint cameraTargetPoint;

    public float speed = 6f;
    public float rotationSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float jumpSpeed = 5f;
    public float groundCheckDistance = 0.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;
    private bool isHit;
    private bool isGrounded;
    RaycastHit groundHit;
    RaycastHit previousGroundHit;

    Vector3 previousHitedObjectPosition;
    Vector3 currentHitedObjectPosition;

    Vector3 lastVelocityOfGroundedObject;
    private bool doubleJumpUsed = false;
    private Animator animator;
    private MaterialSwapper swapper;
    private bool wasRunning;
    Vector3 verticalVelocity;

    float fallingTime = 0f;

    public Vector3 GetMovement()
    {
        return controller.velocity;
    }

    public void Move(Vector3 movement)
    {
        controller.Move(movement);
    }

    public RaycastHit GetGroundHit()
    {
        return groundHit;
    }

    public void SetIsDoubleJumpUsed(bool used)
    {
        doubleJumpUsed = used;
    }



    bool IsGrounded()
    {
        // Get the gravity direction (normalized)
        Vector3 castDirection = Physics.gravity.normalized;
        groundCheckRadius = controller.radius;
        groundCheckDistance = controller.height / 2f - controller.radius + 0.1f;

        Vector3 castOrigin = transform.position;
        previousGroundHit = groundHit;
        previousHitedObjectPosition = currentHitedObjectPosition;
        isHit = Physics.SphereCast(castOrigin, groundCheckRadius, castDirection, out groundHit, groundCheckDistance, groundMask);

        if (isHit)
        {
            currentHitedObjectPosition = groundHit.collider.transform.position;
        }
        return isHit;
    }

    bool canMoveUpwards()
    {
        Vector3 castDirection = Physics.gravity.normalized * -1f;
        groundCheckRadius = controller.radius;
        groundCheckDistance = controller.height / 2f - controller.radius + 0.1f;

        Vector3 castOrigin = transform.position;

        bool hitUpwards = Physics.SphereCast(castOrigin, groundCheckRadius, castDirection, out RaycastHit hitUp, groundCheckDistance, groundMask);
        if (hitUpwards)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

    private void OnDrawGizmos()
    {
        // Only draw in the editor
        if (Application.isPlaying) 
        {
            // Set the color for the cast
            Gizmos.color = Color.yellow; 

            // Draw the start sphere
            //Gizmos.DrawWireSphere(transform.position, groundCheckRadius);

            if (isHit)
            {
                // If hit, change color to visualize success
                Gizmos.color = Color.red; 

                // Calculate the center of the sphere at the moment of impact
                Vector3 hitSphereCenter = transform.position + Physics.gravity.normalized * groundHit.distance;

                // Draw the sphere at the hit point
                Gizmos.DrawWireSphere(hitSphereCenter, groundCheckRadius);

                // Draw a line segment to show the sweep path up to the hit
                Gizmos.DrawLine(transform.position, hitSphereCenter);

                // Optional: Draw a point at the impact position
                Gizmos.DrawSphere(groundHit.point, 0.05f);

            }
            else
            {
                Gizmos.color = Color.green;
                // If no hit, draw the full cast distance (from start sphere's center)
                Vector3 endPosition = transform.position + Physics.gravity.normalized * (controller.height / 2f - controller.radius + 0.1f);
                Gizmos.DrawLine(transform.position, endPosition);
                
                // Draw a wire sphere at the max distance to show the sweep end
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

    // Update is called once per frame
    void Update()
    {

        Vector2 direction2d = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        Vector3 direction = new Vector3(direction2d.x, 0f, direction2d.y).normalized;

        Vector3 velocity = Vector3.zero;

        isGrounded = IsGrounded();

        // Apply gravity
        if (isGrounded && Vector3.Dot(verticalVelocity, Physics.gravity.normalized) > 0)
        {
            verticalVelocity = Physics.gravity / 3f; // Small value to keep grounded
            doubleJumpUsed = false;
            fallingTime = 0f;
        }



        // Jump
        if (InputSystem.actions.FindAction("Jump").triggered && isGrounded)
        {
            verticalVelocity = -Physics.gravity.normalized * jumpSpeed;
	        animator.SetTrigger("jump");
        }
        else if (InputSystem.actions.FindAction("Jump").triggered && !isGrounded && !doubleJumpUsed)
        {
            verticalVelocity = -jumpSpeed * Physics.gravity.normalized;
            doubleJumpUsed = true;
        }


        //Sprint
        if (InputSystem.actions.FindAction("Sprint").IsPressed())
        {
            speed = 10f;
        }
        else
        {
            speed = 6f;
        }



        if (!isGrounded)
        {
            fallingTime += Time.deltaTime;
            verticalVelocity += Physics.gravity * Time.deltaTime;
        }
       
        // Apply vertical movement (gravity and jump)
        if (Vector3.Dot(verticalVelocity, Physics.gravity) < 0 && !canMoveUpwards())
        {
            verticalVelocity = Vector3.zero;
        }
        // print(verticalVelocity);
        //controller.Move(verticalVelocity * Time.deltaTime);
        velocity += verticalVelocity * Time.deltaTime;

        


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Preserve the current X and Z rotation, only update Y rotation
            Vector3 currentRotation = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotation.x, angle, currentRotation.z);


            float inAirCorrection = 1f;
            if (!isGrounded)
            {
                inAirCorrection = 0.5f;
            }
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            velocity += moveDir * (speed * Time.deltaTime * inAirCorrection);
        }
        controller.Move(velocity);

        // print(velocity);
        if(InputSystem.actions.FindAction("Attack").IsPressed()){
            Cursor.visible = !Cursor.visible;
        }
        setAnimation(direction.magnitude);
    }

    private void setAnimation(float magnitude)
    {
        bool isRunning = InputSystem.actions.FindAction("Sprint").IsPressed();

        if (isRunning != wasRunning)
        {
            print("Changing eye material");
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
        if(!isGrounded && fallingTime > 0.2f)
        {
            animator.SetBool("isInAir", true);
        }else
        {
            animator.SetBool("isInAir", false);
        }

        float animationSpeed = 0f;
        if (magnitude >= 0.1f)
        {
            animationSpeed = InputSystem.actions.FindAction("Sprint").IsPressed() ? 2f : 1f;
        }
        animator.SetFloat("Speed", animationSpeed);
    }
}