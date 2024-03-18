using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    float horizontalSensitivity = 10.0f;

    private Transform playerTransform;
    private MainInputActions inputActions;
    private Vector2 mouseInput;
    private Vector3 cameraRotation;
    private float initialCameraOffset;
    private float initialCameraHeight;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialCameraOffset = Vector3.Distance(transform.position, playerTransform.position);
        initialCameraHeight = transform.position.y;
        cameraRotation = transform.rotation.eulerAngles;

        inputActions.General.RotateCamera.performed += context => mouseInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (mouseInput != Vector2.zero)
        {
            // modify our camera rotation angle on Y axis with mouse X change
            cameraRotation.y += mouseInput.x * horizontalSensitivity;
            mouseInput = Vector2.zero;
        }

        // rotate camera
        transform.eulerAngles = cameraRotation;

        // compute camera's look direction
        Vector3 cameraLookDirection = Quaternion.Euler(cameraRotation) * Vector3.forward;

        // position camera pushing back along the look direction
        transform.position = -cameraLookDirection * initialCameraOffset + playerTransform.position;
        transform.position = new Vector3(transform.position.x, initialCameraHeight, transform.position.z);

        // rotate player to have same direction as camera by copying Y rotation angle
        Vector3 playerRotation = playerTransform.eulerAngles;
        playerRotation.y = transform.rotation.eulerAngles.y;
        playerTransform.eulerAngles = playerRotation;
    }

    private void OnEnable()
    {
        inputActions = new MainInputActions();
        inputActions.General.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        inputActions.General.Disable();
        Cursor.lockState = CursorLockMode.None;
    }
}
