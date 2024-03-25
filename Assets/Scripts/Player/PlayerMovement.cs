using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float rotationInterpolationSpeed = 25f;
    [SerializeField] float runSpeed = 500f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float groundDistance = 0.05f;
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] LayerMask groundMask;

    // Input handling
    MainInputActions inputActions;
    Animator animationController;
    Rigidbody rigidBody;
    Vector3 cameraForward = new Vector3();
    Vector3 cameraRight = new Vector3();

    // Movement
    Vector2 inputMovementVector = new Vector2();
    public bool isGrounded = false;
    float jumpCoolDown = 0.5f;
    float timeOfLastJump = 0f;


    void Awake()
    {
        inputActions = new MainInputActions();
        animationController = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        timeOfLastJump = Time.time;
    }

    void Update()
    {
        SaveCameraDirections();
        CheckIsGrounded();

        bool hasJumped = false;
        inputMovementVector = inputActions.General.Movement.ReadValue<Vector2>();

        if (inputMovementVector.magnitude > 0.1f)
        {
            // Calculate the movement direction based on input and character's orientation
            Vector3 movementDirection = new Vector3(inputMovementVector.x, 0, inputMovementVector.y);

            // Calculate the rotation to face the movement direction while aligning with the camera's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up) * Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);

            // Apply the rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationInterpolationSpeed);
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationInterpolationSpeed);
        }

        if (inputActions.General.Jump.WasPressedThisFrame() && CanJump())
        {
            timeOfLastJump = Time.time;
            hasJumped = true;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        UpdateAnimator(hasJumped);
    }

    void FixedUpdate()
    {
        if (inputMovementVector.magnitude > 0.1f)
        {
            Vector3 movementDirection = cameraForward * inputMovementVector.y + cameraRight * inputMovementVector.x;
            float rbVelocityY = rigidBody.velocity.y;
            rigidBody.velocity = movementDirection * runSpeed * Time.fixedDeltaTime;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rbVelocityY, rigidBody.velocity.z);
        }
        else
        {
            rigidBody.velocity = new Vector3(0f, rigidBody.velocity.y, 0f);
        }
    }

    void SaveCameraDirections()
    {
        cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        cameraRight = Camera.main.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
    }

    bool CanJump()
    {
        return isGrounded && timeOfLastJump < Time.time - jumpCoolDown;
    }

    void CheckIsGrounded()
    {
        isGrounded = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundDistance, groundMask);
        Debug.DrawRay(groundCheckOrigin.position, Vector3.down, Color.red, groundDistance);
    }

    void UpdateAnimator(bool hasJumped)
    {
        animationController.SetBool("Running", inputMovementVector.magnitude > 0);
        animationController.SetBool("IsLanded", isGrounded);

        if (hasJumped)
        {
            animationController.SetTrigger("Jump");
        }
    }

    void OnEnable()
    {
        inputActions.General.Enable();
    }

    void OnDisable()
    {
        inputActions.General.Disable();
    }
}
