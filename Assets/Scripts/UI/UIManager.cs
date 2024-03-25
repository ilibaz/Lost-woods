using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveAction
{
    public string actionName;
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
            interactiveActionText.text = currentInteractiveAction.actionName + " " + currentInteractiveAction.itemName;
        }
        else
        {
            interactiveActionText.text = "";
        }
    }
}
