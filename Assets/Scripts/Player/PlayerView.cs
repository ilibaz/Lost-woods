using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private static PlayerView instance;

    public static PlayerView Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerView>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlayerView).Name);
                    instance = singletonObject.AddComponent<PlayerView>();
                }
            }
            return instance;
        }
    }

    Animator animationController;

    void Start()
    {
        animationController = GetComponent<Animator>();
        PlayerMovementController.Instance.OnJump += UpdateAnimatorJump;
        PlayerMovementController.Instance.OnLanded += UpdateAnimatorIsLanded;
        PlayerPickUpController.Instance.OnPickUpItem += UpdateAnimatorPickUp;
        PlayerInventoryController.Instance.OnEquipItemLeftHand += UpdateAnimatorPickUpTorch;
        PlayerInventoryController.Instance.OnEquipItemRightHand += UpdateAnimatorPickUpSword;
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

    void UpdateAnimatorPickUp(ItemView item)
    {
        animationController.SetTrigger("PickUp");
    }


    void UpdateAnimatorPickUpTorch()
    {
        animationController.SetBool("LeftHandTorchEquped", true);
    }

    void UpdateAnimatorPickUpSword()
    {
        //animationController.SetBool("LeftHandSwordEquped", true);
    }
}
