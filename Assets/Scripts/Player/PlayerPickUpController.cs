using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    private static PlayerPickUpController instance;

    public static PlayerPickUpController Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerPickUpController>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlayerPickUpController).Name);
                    instance = singletonObject.AddComponent<PlayerPickUpController>();
                }
            }
            return instance;
        }
    }

    [SerializeField] GameObject pickUpCollider;

    public event Action<ItemView> OnPickUpItem;

    ItemView currentPickableItem;

    MainInputActions inputActions;

    void Awake()
    {
        inputActions = new MainInputActions();
    }

    void Update()
    {
        if (inputActions.General.Interact.WasPerformedThisFrame() && currentPickableItem != null)
        {
            PickUpItem();
        }
    }

    void OnTriggerStay(Collider other)
    {
        ItemView item;
        other.gameObject.TryGetComponent<ItemView>(out item);

        if (item && currentPickableItem == null)
        {
            currentPickableItem = item;
            UIManager.Instance.SetInteractiveAction(item.action);
        }
    }

    void OnTriggerExit(Collider other)
    {
        ItemView item;
        other.gameObject.TryGetComponent<ItemView>(out item);

        if (item)
        {
            UIManager.Instance.ClearInteractiveAction();

            if (currentPickableItem == item)
            {
                currentPickableItem = null;
            }
        }
    }

    void PickUpItem()
    {
        if (OnPickUpItem != null)
        {
            OnPickUpItem.Invoke(currentPickableItem);
        }

        UIManager.Instance.ClearInteractiveAction();
        currentPickableItem = null;
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
