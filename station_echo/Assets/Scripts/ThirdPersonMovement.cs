using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CharacterController controller;
    public Transform cam;
    public Transform mesh;
    public Material eyes_idle;
    public Material eyes_run;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float jumpForce = 2f;

    private bool doubleJumpUsed = false;

    Vector3 velocity;

    private Animator animator;

    void Start()
    {
	animator = mesh.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 direction2d = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        Vector3 direction = new Vector3(direction2d.x, 0f, direction2d.y).normalized;

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = Physics.gravity.y / 3f; // Small value to keep grounded
            doubleJumpUsed = false;
        }


        // Jump
        if (InputSystem.actions.FindAction("Jump").triggered && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
        else if (InputSystem.actions.FindAction("Jump").triggered && !controller.isGrounded && !doubleJumpUsed)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            doubleJumpUsed = true;
        }


        // Apply gravity over time
        velocity += Physics.gravity * Time.deltaTime;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            float inAirCorrection = 1f;
            if (!controller.isGrounded)
            {
                inAirCorrection = 0.5f;
            }
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
		
	    float actualSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(moveDir * (actualSpeed * Time.deltaTime * inAirCorrection));
        };
        // Apply vertical movement (gravity and jump)
        controller.Move(velocity * Time.deltaTime);

	setAnimation(direction.magnitude);
    }

    private void setAnimation(float magnitude)
    {
	float animationSpeed = 0;	
	
    	if (magnitude >= 0.1)
    	{
            animationSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        }

        animator.SetFloat("Speed", animationSpeed);
	Debug.Log($"speed: {animationSpeed}");
    }
}
