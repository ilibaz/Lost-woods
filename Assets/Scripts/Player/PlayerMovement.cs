using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float rotationInterpolationSpeed = 25f;
    [SerializeField] float runSpeed = 500f;

    MainInputActions inputActions;
    Animator animationController;
    Rigidbody rigidBody;
    Vector2 inputMovementVector = new Vector2();
    Vector3 cameraForward = new Vector3();
    Vector3 cameraRight = new Vector3();


    void Awake()
    {
        inputActions = new MainInputActions();
        animationController = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
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

        UpdateAnimator();
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

    void UpdateAnimator()
    {
        animationController.SetBool("Running", inputMovementVector.magnitude > 0);
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
