using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    MainInputActions inputActions;
    Animator animationController;
    Rigidbody rigidbody;
    Vector2 inputMovementVector = new Vector2();

    [SerializeField] float speed = 1f;

    void Awake()
    {
        inputActions = new MainInputActions();
        animationController = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {
        inputMovementVector = inputActions.General.Movement.ReadValue<Vector2>();
        animationController.SetFloat("SpeedX", inputMovementVector.x);
        animationController.SetFloat("SpeedY", inputMovementVector.y);
        animationController.SetBool("Running", inputMovementVector.magnitude > 0);
    }

    void FixedUpdate()
    {
        rigidbody.velocity = new Vector3(inputMovementVector.x * speed, rigidbody.velocity.y, inputMovementVector.y * speed);
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
