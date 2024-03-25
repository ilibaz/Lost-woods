using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] string itemName = "Name";
    [SerializeField] public bool autoEquibale;
    [SerializeField] public bool leftHanded;
    [SerializeField] public Vector3 positionAfterPickUp;
    [SerializeField] public Vector3 rotationAfterPickUp;

    public InteractiveAction action;

    Rigidbody rb;
    Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        action = new InteractiveAction();
        action.itemName = itemName;
        action.actionName = "Pick up";
    }

    public void PickUp()
    {
        // spawn particle system if needed
    }
}
