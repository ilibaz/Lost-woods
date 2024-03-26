using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    private static PlayerInventoryController instance;

    public static PlayerInventoryController Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerInventoryController>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlayerInventoryController).Name);
                    instance = singletonObject.AddComponent<PlayerInventoryController>();
                }
            }
            return instance;
        }
    }

    [SerializeField] Transform playerLeftArmHand;
    [SerializeField] Transform playerRightArmHand;

    public event Action OnEquipItemLeftHand;
    public event Action OnEquipItemRightHand;

    void Start()
    {
        PlayerPickUpController.Instance.OnPickUpItem += PickUpItem;
    }

    private void PickUpItem(ItemView item)
    {
        if (item.autoEquibale)
        {
            if (item.leftHanded)
            {
                StartCoroutine(EquipItemToLeftHandAfter(item, PlayerMovementController.Instance.pickUpMovementCooldown));
            }
            else
            {
                StartCoroutine(EquipItemToRightHandAfter(item, PlayerMovementController.Instance.pickUpMovementCooldown));
            }
        }
        else
        {
            // put it to inventory
        }

        item.PickUp();
    }

    IEnumerator EquipItemToLeftHandAfter(ItemView item, float t)
    {
        EquipToHand(playerLeftArmHand, item);
        yield return new WaitForSeconds(t);
        item.gameObject.SetActive(true);
        EnableLights(item.gameObject);

        if (OnEquipItemLeftHand != null)
        {
            OnEquipItemLeftHand.Invoke();
        }
    }

    IEnumerator EquipItemToRightHandAfter(ItemView item, float t)
    {
        EquipToHand(playerRightArmHand, item);
        yield return new WaitForSeconds(t);
        item.gameObject.SetActive(true);
        EnableLights(item.gameObject);

        if (OnEquipItemRightHand != null)
        {
            OnEquipItemRightHand.Invoke();
        }
    }

    void EquipToHand(Transform hand, ItemView item)
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
}
