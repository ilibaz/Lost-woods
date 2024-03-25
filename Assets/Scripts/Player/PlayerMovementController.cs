using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private static PlayerMovementController instance;

    public static PlayerMovementController Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerMovementController>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlayerMovementController).Name);
                    instance = singletonObject.AddComponent<PlayerMovementController>();
                }
            }
            return instance;
        }
    }

    [SerializeField] float rotationInterpolationSpeed = 25f;
    [SerializeField] float runSpeed = 500f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float groundCheckDistance = 0.05f;
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] LayerMask groundMask;

    // Input handling
    MainInputActions inputActions;
    Rigidbody rigidBody;
    Vector3 cameraForward = new Vector3();
    Vector3 cameraRight = new Vector3();

    // Movement
    public event Action OnJump;
    public event Action OnLanded;
    public Vector2 inputMovementVector { get; private set; }
    public bool isGrounded { get; private set; }
    public bool isJumping { get; private set; }
    public bool canMove;
    float jumpCoolDown = 0.5f;
    float timeOfLastJump = 0f;
    float previousGroundDistance = 0f;


    void Awake()
    {
        inputActions = new MainInputActions();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        timeOfLastJump = Time.time;
        canMove = true;

        PlayerPickUpManager.Instance.OnEquipTorch += DisableMovementWhileInteracting;
    }

    void Update()
    {
        if (canMove)
        {
            SaveCameraDirections();
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
                isJumping = true;
                timeOfLastJump = Time.time;
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                if (OnJump != null)
                {
                    OnJump.Invoke();
                }
            }
        }

        CheckIsGrounded();
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
        return isGrounded && !isJumping && timeOfLastJump < Time.time - jumpCoolDown;
    }

    void CheckIsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheckOrigin.position, Vector3.down, out hit, groundCheckDistance, groundMask))
        {
            float distance = Mathf.Round(hit.distance * 1000) / 1000;

            // there is a surface hit but we are jumping up now
            if (isJumping && previousGroundDistance < distance)
            {
                isGrounded = false;
            }
            // there is a surface hit but we are falling down from jump now
            else if (isJumping && previousGroundDistance > distance)
            {
                isGrounded = true;
                isJumping = false;

                if (OnLanded != null)
                {
                    OnLanded.Invoke();
                }
            }
            // there is a surface hit and we aren't jumping
            else if (!isJumping)
            {
                isGrounded = true;

                if (OnLanded != null)
                {
                    OnLanded.Invoke();
                }
            }

            previousGroundDistance = distance;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(groundCheckOrigin.position, Vector3.down, Color.red, groundCheckDistance);
    }

    void DisableMovementWhileInteracting()
    {
        canMove = false;

        StartCoroutine(EnableMovementAfter(0.75f));
    }

    IEnumerator EnableMovementAfter(float t)
    {
        yield return new WaitForSeconds(t);
        canMove = true;
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
