using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    float sensitivity = 1.0f;
    [SerializeField]
    float minVerticalRotation = 15f;
    [SerializeField]
    float maxVerticalRotation = 35f;
    [SerializeField]
    Vector3 playerHeightOffset = new Vector3(0, 0.5f, 0);
    [SerializeField]
    LayerMask obstacleMask;

    private Transform playerTransform;
    private MainInputActions inputActions;
    private Vector2 mouseInput;
    private Vector3 cameraRotation;
    private float initialCameraOffset;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialCameraOffset = Vector3.Distance(transform.position, playerTransform.position);
        cameraRotation = transform.rotation.eulerAngles;

        inputActions.General.RotateCamera.performed += context => mouseInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (mouseInput != Vector2.zero)
        {
            // modify our camera rotation angle on Y axis with mouse X change
            cameraRotation.y += mouseInput.x * sensitivity;
            cameraRotation.x += -mouseInput.y * sensitivity;
            cameraRotation.x = Mathf.Clamp(cameraRotation.x, minVerticalRotation, maxVerticalRotation);
            mouseInput = Vector2.zero;
        }

        // rotate camera horizontally
        transform.eulerAngles = new Vector3(cameraRotation.x, cameraRotation.y, transform.eulerAngles.z);

        // compute camera's look direction
        Vector3 cameraLookDirection = Quaternion.Euler(transform.eulerAngles) * Vector3.forward;

        // position camera pushing back along the look direction
        Vector3 desiredPosition = -cameraLookDirection * initialCameraOffset + playerTransform.position + playerHeightOffset;

        // check for collisions with objects on the way
        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, desiredPosition - playerTransform.position, out hit, initialCameraOffset, obstacleMask))
        {
            // Adjust camera position to be just in front of obstacle
            Vector3 towardsPlayer = (transform.position - hit.point).normalized;
            desiredPosition = hit.point + new Vector3(towardsPlayer.x, towardsPlayer.y * 1.5f, towardsPlayer.z);
        }

        transform.position = desiredPosition;
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
