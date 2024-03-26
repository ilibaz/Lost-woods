using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ActionType
{
    PickUp,
    Harvest,
    Interact
}

public class InteractiveAction
{
    public ActionType actionType;
    public string itemName;
}

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            // If the instance doesn't exist yet, find it in the scene
            if (instance == null)
            {
                instance = FindFirstObjectByType<UIManager>();

                // If it's still null, create a new instance
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UIManager).Name);
                    instance = singletonObject.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] TMP_Text interactiveActionText;

    InteractiveAction currentInteractiveAction;

    void Update()
    {
        RenderInteractiveAction();
    }

    public void SetInteractiveAction(InteractiveAction action)
    {
        currentInteractiveAction = action;
    }

    public void ClearInteractiveAction()
    {
        currentInteractiveAction = null;
    }

    void RenderInteractiveAction()
    {
        if (currentInteractiveAction != null)
        {
            interactiveActionText.text = GetActionText(currentInteractiveAction.actionType) + " " + currentInteractiveAction.itemName;
        }
        else
        {
            interactiveActionText.text = "";
        }
    }

    string GetActionText(ActionType action)
    {
        switch (action)
        {
            case ActionType.PickUp:
                return "Pick up";
            case ActionType.Harvest:
                return "Harvest";
            case ActionType.Interact:
                return "Interact";
            default:
                return "ACTION TEXT ERROR";
        }
    }
}
