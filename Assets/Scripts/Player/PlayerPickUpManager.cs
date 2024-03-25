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
    [SerializeField] Transform playerLeftArmHand;
    [SerializeField] Transform playerRightArmHand;


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
            PickUpItem();
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

    void PickUpItem()
    {
        if (OnPickUpItem != null)
        {
            OnPickUpItem.Invoke();
        }

        if (currentPickableItem.autoEquibale)
        {
            if (currentPickableItem.leftHanded)
            {
                StartCoroutine(EquipItemToLeftHandAfter(currentPickableItem, PlayerMovementController.Instance.pickUpMovementCooldown));
            }
            else
            {
                StartCoroutine(EquipItemToRightHandAfter(currentPickableItem, PlayerMovementController.Instance.pickUpMovementCooldown));
            }
        }
        else
        {
            // put it to inventory
        }

        UIManager.Instance.ClearInteractiveAction();
        currentPickableItem.PickUp();
        currentPickableItem = null;
    }

    IEnumerator EquipItemToLeftHandAfter(PickableItem item, float t)
    {
        EquipToHand(playerLeftArmHand, item);
        yield return new WaitForSeconds(t);
        item.gameObject.SetActive(true);
        EnableLights(item.gameObject);
    }

    IEnumerator EquipItemToRightHandAfter(PickableItem item, float t)
    {
        EquipToHand(playerRightArmHand, item);
        yield return new WaitForSeconds(t);
        item.gameObject.SetActive(true);
        EnableLights(item.gameObject);
    }

    void EquipToHand(Transform hand, PickableItem item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(hand);

        item.transform.localPosition = item.positionAfterPickUp;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localRotation *= Quaternion.Euler(item.rotationAfterPickUp);

        Rigidbody itemRb;
        item.TryGetComponent<Rigidbody>(out itemRb);
        if (itemRb)
        {
            Destroy(itemRb);
        }
    }

    void EnableLights(GameObject item)
    {
        Light[] lights = item.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
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
