using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[DefaultExecutionOrder(-25)]
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
    private float GroundedTimestamp = 0f;

    bool wasMaterialChanged = false;
    RaycastHit groundHit;
    RaycastHit previousGroundHit;

    Vector3 previousHitedObjectPosition;
    Vector3 currentHitedObjectPosition;

    [SerializeField] float baseHorizontalAcceleration = 10f;
    [SerializeField] float midAirHorizontalAcceleration = 5f;
    [SerializeField] float horizontalDeceleration = 15f;
    [SerializeField] float maxRunSpeed = 6f;
    [SerializeField] float maxWalkSpeed = 3f;
    float currentMaxSpeed = 0f;
    private bool doubleJumpUsed = false;
    private Animator animator;
    private MaterialSwapper swapper;
    Vector3 verticalVelocity = Vector3.zero;
    Vector3 ownHorizontalVelocity = Vector3.zero;
    RaycastHit hitForward;
    Vector3 givenHorizontalVelocity = Vector3.zero;

    private Vector2 _direction2d;
    private bool _sprintHeld;

    private bool wasSprinting = false;
    private bool _jumpTriggered;
    bool isAllowedToControl = true;

    private MoveableObject moveableObject;

    public float maxFallSpeed = float.MaxValue;

    float fallingTime = 0f;

    public Vector3 GetVelocity()
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
        groundCheckDistance = controller.height / 2f - controller.radius + 0.15f;

        Vector3 castOrigin = transform.position;
        previousGroundHit = groundHit;
        previousHitedObjectPosition = currentHitedObjectPosition;
        isHit = Physics.SphereCast(castOrigin, groundCheckRadius, castDirection, out groundHit, groundCheckDistance, groundMask);

        if (isHit)
        {
            currentHitedObjectPosition = groundHit.collider.transform.position;
            GroundedTimestamp = Time.time;
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

    bool CheckForward()
    {
        Vector3 horizontalVelocity = ownHorizontalVelocity + givenHorizontalVelocity;

        // 1. Handle zero velocity to prevent errors
        if (horizontalVelocity.sqrMagnitude < 0.01f) return false;

        Vector3 castDir = horizontalVelocity.normalized;
        
        // 2. Convert local controller center to world space
        Vector3 worldCenter = transform.TransformPoint(controller.center);

        // 3. Calculate distance from center to the hemispherical ends
        // The height is the total height; we subtract two radii to find the cylinder part
        float halfCylinderHeight = (controller.height * 0.5f) - controller.radius;

        // 4. Define the top and bottom spheres of the capsule
        Vector3 pointTop = worldCenter + transform.up * halfCylinderHeight;
        Vector3 pointBottom = worldCenter - transform.up * halfCylinderHeight;

        // 5. Cast slightly further than the radius to detect upcoming walls
        float castDistance = 0.2f; 

        bool isHitForward = Physics.CapsuleCast(
            pointTop, 
            pointBottom, 
            controller.radius, 
            castDir, 
            out hitForward, 
            castDistance, 
            groundMask
        );

        return isHitForward;
    }

    Vector3 SubtractOppositeComponent(Vector3 velocity, Vector3 wind)
    {
        // 1. Project wind onto the velocity direction
        // This gives us the portion of wind acting along the velocity's axis
        Vector3 projection = Vector3.Project(wind, velocity);

        // 2. Check if the projection is opposite to the velocity
        // If the Dot product is less than 0, they are facing away from each other
        if (Vector3.Dot(velocity, projection) < 0)
        {
            // 3. Subtract that opposite component
            // Subtracting a negative-facing vector effectively "cancels" that force
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, projection.magnitude);
        }

        // If the vector isn't opposite, return the original velocity
        return velocity;
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


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveableObject = GetComponent<MoveableObject>();

        animator = mesh.GetComponent<Animator>();
        swapper = mesh.GetComponentInChildren<MaterialSwapper>();
    }

    void Start()
    {
        cameraTargetPoint = transform.GetComponentInChildren<CameraTargetPoint>();
    }





    // Update is called once per frame
    void Update()
    {

        _direction2d = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        _sprintHeld = InputSystem.actions.FindAction("Sprint").IsPressed();


        if (InputSystem.actions.FindAction("Jump").triggered)
        {
            _jumpTriggered = true;
        }

        if (InputSystem.actions.FindAction("Attack").IsPressed())
        {
            Cursor.visible = !Cursor.visible;
        }
    }

    void FixedUpdate()
    {
        Vector3 platformDelta = moveableObject.GetPropagationMovement();

        isGrounded = IsGrounded();


        if (isGrounded && Vector3.Dot(verticalVelocity, Physics.gravity.normalized) > 0)
        {
            verticalVelocity = Physics.gravity / 3f;
            doubleJumpUsed = false;
            fallingTime = 0f;
        }

        if (_jumpTriggered)
        {
            if (Time.time - GroundedTimestamp < 0.2f)
            {
                verticalVelocity = -Physics.gravity.normalized * jumpSpeed;
                animator.SetTrigger("jump");
            }
            else if (!(Time.time - GroundedTimestamp < 0.2f) && !doubleJumpUsed)
            {
                verticalVelocity = -jumpSpeed * Physics.gravity.normalized;
                doubleJumpUsed = true;
            }
            _jumpTriggered = false;
        }


        if (!isGrounded)
        {
            fallingTime += Time.fixedDeltaTime;
            verticalVelocity += Physics.gravity * Time.fixedDeltaTime;
            // Used by trigger in LVL4
            if (verticalVelocity.y < -maxFallSpeed)
            {
                verticalVelocity.y = -maxFallSpeed;
            }
        }

        if(CheckForward())
        {
            // 1. Get the normal of the surface we hit
            Vector3 surfaceNormal = hitForward.normal;

            // 2. Find the 'dot product' - this is the magnitude of velocity 
            // pointing directly into the surface normal
            float dotOwn = Vector3.Dot(ownHorizontalVelocity, surfaceNormal);
            float dotGiven = Vector3.Dot(givenHorizontalVelocity, surfaceNormal);
            // 3. If the dot product is negative, it means we are moving TOWARD the surface
            if (dotOwn < 0)
            {
                // 4. Calculate the 'rejection' vector (the part of velocity hitting the wall)
                Vector3 velocityIntoWall = surfaceNormal * dotOwn;

                // 5. Subtract it from the current velocity to get the sliding velocity
                ownHorizontalVelocity -= velocityIntoWall;
            }

            if (dotGiven < 0)
            {
                // 4. Calculate the 'rejection' vector (the part of velocity hitting the wall)
                Vector3 velocityIntoWall = surfaceNormal * dotGiven;

                // 5. Subtract it from the current velocity to get the sliding velocity
                givenHorizontalVelocity -= velocityIntoWall;
            }
        }

        if (isGrounded){
            wasSprinting = _sprintHeld;
            isAllowedToControl = true;
        }

        if (Vector3.Dot(verticalVelocity, Physics.gravity) < 0 && !canMoveUpwards())
        {
            verticalVelocity = Vector3.zero;
        }

        Vector3 verticalDelta = verticalVelocity * Time.fixedDeltaTime;


        Vector3 direction = new Vector3(_direction2d.x, 0f, _direction2d.y).normalized;
        Vector3 horizontalDelta = Vector3.zero;

        direction.x *= cameraTargetPoint.currentGravityDirection;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y ;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            Vector3 currentRotation = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotation.x, angle, currentRotation.z);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            CalculateCurrentMaxSpeed(isGrounded);
            CalculateHorizontalVelocity(moveDir, isGrounded);
            horizontalDelta = ownHorizontalVelocity * Time.fixedDeltaTime;
            if(isGrounded) givenHorizontalVelocity = Vector3.MoveTowards(givenHorizontalVelocity, Vector3.zero, horizontalDeceleration * Time.fixedDeltaTime);
            givenHorizontalVelocity = SubtractOppositeComponent(givenHorizontalVelocity, ownHorizontalVelocity*Time.fixedDeltaTime);
        }
        else
        {
            if (isGrounded)
            {
                givenHorizontalVelocity = Vector3.MoveTowards(givenHorizontalVelocity, Vector3.zero, horizontalDeceleration * Time.fixedDeltaTime);
                ownHorizontalVelocity = Vector3.MoveTowards(ownHorizontalVelocity, Vector3.zero, horizontalDeceleration * Time.fixedDeltaTime);
                horizontalDelta = ownHorizontalVelocity * Time.fixedDeltaTime;
            }else{
                
                ownHorizontalVelocity = Vector3.MoveTowards(ownHorizontalVelocity, Vector3.zero, (midAirHorizontalAcceleration) * Time.fixedDeltaTime);
                horizontalDelta = ownHorizontalVelocity * Time.fixedDeltaTime;
            }
        }

        
        // print(" givenHorizontalVelocity: " + givenHorizontalVelocity);
        // print(" ownHorizontalVelocity: " + ownHorizontalVelocity);
        Vector3 finalDelta = horizontalDelta + verticalDelta + platformDelta + givenHorizontalVelocity * Time.fixedDeltaTime;
        controller.Move(finalDelta);

        setAnimation(direction.magnitude, _sprintHeld);
    }

    private void CalculateCurrentMaxSpeed(bool isGrounded){
        if (isGrounded){
            if (_sprintHeld){
                currentMaxSpeed = maxRunSpeed;
            }
            else{
                currentMaxSpeed = maxWalkSpeed;
            }
        }
        else{
            if (wasSprinting){
                currentMaxSpeed = Mathf.MoveTowards(currentMaxSpeed, maxWalkSpeed*1.2f, 60 * Time.fixedDeltaTime);
            }
            else{
                currentMaxSpeed = Mathf.MoveTowards(currentMaxSpeed, maxWalkSpeed*0.6f, 60 * Time.fixedDeltaTime);
            }
            
        }
    }

    public void AddVelocity(Vector3 velocityToAdd)
    {
        givenHorizontalVelocity = new Vector3(givenHorizontalVelocity.x + velocityToAdd.x, givenHorizontalVelocity.y, givenHorizontalVelocity.z + velocityToAdd.z);
        verticalVelocity = new Vector3(verticalVelocity.x, verticalVelocity.y + velocityToAdd.y, verticalVelocity.z);  
    }

    public void SetAllowedToControl(bool allowed)
    {
        isAllowedToControl = allowed;
    }

    private void CalculateHorizontalVelocity(Vector3 moveDir, bool isGrounded){
        Vector3 targetVelocity = moveDir * currentMaxSpeed;
        float acceleration = isGrounded ? baseHorizontalAcceleration : midAirHorizontalAcceleration;
        ownHorizontalVelocity = Vector3.MoveTowards(ownHorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void setAnimation(float magnitude, bool isRunning)
    {
        if (isRunning && magnitude >= 0.1f)
        {
            if (wasMaterialChanged == false)
            {
                swapper.SetMaterial(0, "eyes_run");
                wasMaterialChanged = true;
            }
        }
        else
        {
            swapper.SetMaterial(0, "eyes_idle");
            wasMaterialChanged = false;
        }

        if (!isGrounded && fallingTime > 0.2f)
        {
            animator.SetBool("isInAir", true);
        }
        else
        {
            animator.SetBool("isInAir", false);
        }

        float animationSpeed = 0f;
        if (magnitude >= 0.1f)
        {
            animationSpeed = isRunning ? 2f : 1f;
        }
        animator.SetFloat("Speed", animationSpeed);
    }
}