using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CharacterController controller;
    public Transform cam;
    public Transform mesh;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float jumpForce = 0.5f;
    public float groundCheckDistance = 0.2f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;
    private bool isHit;
    RaycastHit hit;
    private bool doubleJumpUsed = false;
    private Animator animator;

    Vector3 velocity;

    bool IsGrounded()
    {
        // Get the gravity direction (normalized)
        Vector3 gravityDirection = Physics.gravity.normalized;
        groundCheckRadius = controller.radius;
        groundCheckDistance = controller.height / 2f - controller.radius + 0.1f;

        Vector3 castOrigin = transform.position;

        isHit = Physics.SphereCast(castOrigin, groundCheckRadius, gravityDirection, out hit, groundCheckDistance, groundMask);
        if (isHit){
            return true;
        }
        else{
            return false;
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
                Vector3 hitSphereCenter = transform.position + Physics.gravity.normalized * hit.distance;

                // Draw the sphere at the hit point
                Gizmos.DrawWireSphere(hitSphereCenter, groundCheckRadius);

                // Draw a line segment to show the sweep path up to the hit
                Gizmos.DrawLine(transform.position, hitSphereCenter);

                // Optional: Draw a point at the impact position
                Gizmos.DrawSphere(hit.point, 0.05f);

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
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 direction2d = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        Vector3 direction = new Vector3(direction2d.x, 0f, direction2d.y).normalized;

        bool isGrounded = IsGrounded();

        // Apply gravity
        if (isGrounded && Vector3.Dot(velocity, Physics.gravity.normalized) > 0)
        {
            velocity = Physics.gravity.normalized * (Physics.gravity.magnitude / 3f); // Small value to keep grounded
            doubleJumpUsed = false;
        }



        // Jump
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


        //Sprint
        if (InputSystem.actions.FindAction("Sprint").IsPressed())
        {
            speed = 10f;
        }
        else
        {
            speed = 6f;
        }


        // Apply gravity over time
        velocity += Physics.gravity * Time.deltaTime;

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
            controller.Move(moveDir * (speed * Time.deltaTime * inAirCorrection));
        }

        // print(velocity);
        // Apply vertical movement (gravity and jump)
        controller.Move(velocity * Time.deltaTime);
        setAnimation(direction.magnitude);
    }

    private void setAnimation(float magnitude)
    {
	if(IsGrounded())
	{
	    animator.SetBool("isInAir", false);
	}else
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

