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
        PlayerPickUpManager.Instance.OnEquipTorch += UpdateAnimatorGrabTorch;
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

    void UpdateAnimatorGrabTorch()
    {
        animationController.SetTrigger("GrabTorch");
        animationController.SetBool("HasTorch", true);
    }

}
