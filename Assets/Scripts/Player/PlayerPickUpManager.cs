using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpManager : MonoBehaviour
{
    private static PlayerPickUpManager instance;

    public static PlayerPickUpManager Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerPickUpManager>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlayerPickUpManager).Name);
                    instance = singletonObject.AddComponent<PlayerPickUpManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] GameObject pickUpCollider;
    [SerializeField] GameObject playerLeftArmTorch;


    public event Action OnPickUpItem;

    PickableItem currentPickableItem;

    MainInputActions inputActions;

    void Awake()
    {
        inputActions = new MainInputActions();
    }

    void Update()
    {
        if (inputActions.General.Interact.WasPerformedThisFrame())
        {
            if (currentPickableItem.autoEquibale)
            {
                // just torch equip for now
                if (OnPickUpItem != null)
                {
                    OnPickUpItem.Invoke();
                    StartCoroutine(ShowTorchAfter(PlayerMovementController.Instance.pickUpMovementCooldown));
                }
            }

            UIManager.Instance.ClearInteractiveAction();
            currentPickableItem.PickUp();
            currentPickableItem = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        PickableItem item;
        other.gameObject.TryGetComponent<PickableItem>(out item);

        if (item && currentPickableItem == null)
        {
            currentPickableItem = item;
            UIManager.Instance.SetInteractiveAction(item.action);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PickableItem item;
        other.gameObject.TryGetComponent<PickableItem>(out item);

        if (item)
        {
            UIManager.Instance.ClearInteractiveAction();

            if (currentPickableItem == item)
            {
                currentPickableItem = null;
            }
        }
    }

    IEnumerator ShowTorchAfter(float t)
    {
        yield return new WaitForSeconds(t);
        playerLeftArmTorch.SetActive(true);
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
