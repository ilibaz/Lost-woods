using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementView : MonoBehaviour
{
    Animator animationController;

    void Start()
    {
        animationController = GetComponent<Animator>();
        PlayerMovementController.Instance.OnJump += UpdateAnimatorJump;
        PlayerMovementController.Instance.OnLanded += UpdateAnimatorIsLanded;
        PlayerPickUpManager.Instance.OnPickUpItem += UpdateAnimatorPickUpTorch;
    }


    void Update()
    {
        UpdateAnimatorMovement();
    }

    void UpdateAnimatorMovement()
    {

        animationController.SetBool("Running", PlayerMovementController.Instance.inputMovementVector.magnitude > 0);
    }

    void UpdateAnimatorJump()
    {
        animationController.SetTrigger("Jump");
        animationController.SetBool("IsLanded", false);
    }

    void UpdateAnimatorIsLanded()
    {
        animationController.SetBool("IsLanded", true);
    }

    void UpdateAnimatorPickUpTorch()
    {
        animationController.SetTrigger("PickUp");
        animationController.SetBool("LeftHandTorchEquped", true);
    }

}
