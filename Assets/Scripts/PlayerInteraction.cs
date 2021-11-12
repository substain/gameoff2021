using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const string INTERACTION_LMASK_NAME = "Interactable";

    [SerializeField]
    private GameObject interactionPosition;

    [SerializeField]
    private float interactionRadius = 2f;

    [SerializeField]
    private int numberOfBugs = 0;

    [SerializeField]
    private bool displayInteractionSphereGizmo = true;

    [SerializeField]
    private List<int> obtainedKeyIds = new List<int>();

    private IInteractable currentInteractable;

    private LayerMask layerToCheckFor;
    void Awake()
    {
        if(interactionPosition == null)
        {
            Debug.LogWarning("no interactionPosition assigned!");
        }

        layerToCheckFor = LayerMask.GetMask(INTERACTION_LMASK_NAME);
        HUDManager.Instance.SetNumberOfBugs(numberOfBugs);
        HUDManager.Instance.SetObtainedKeys(obtainedKeyIds);
    }

    void Start()
    {
        InvokeRepeating("CheckForInteractables", 0.1f, 0.1f);
    }

    private void CheckForInteractables()
    {
        IInteractable previousInteractable = currentInteractable;
        Collider[] matchingColliders = Physics.OverlapSphere(interactionPosition.transform.position, interactionRadius, layerToCheckFor);
        List<IInteractable> interactables = new List<IInteractable>();

        foreach (Collider col in matchingColliders)
        {
            IInteractable interactable = col.gameObject.GetComponent<IInteractable>();
            if(interactable != null)
            {
                interactables.Add(interactable);
            }
        }

        if(interactables.Count > 0)
        {
            currentInteractable = interactables[0];
            HUDManager.Instance.UpdateActionHintText("Press E to " + currentInteractable.GetInteractionTypeString());
        }
        else
        {
            currentInteractable = null;
            if(previousInteractable != null)
            {
                HUDManager.Instance.UpdateActionHintText("");
            }
        }
    }

    public void ProcessInteractInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (currentInteractable == null)
        {
            return;
        }

        currentInteractable.Interact(this);
    }

    public void RemoveBugFrom(BugAttachment bugAttachment)
    {
        bugAttachment.RemoveBug();
        numberOfBugs++;
    }

    public void PutBugOn(BugAttachment bugAttachment)
    {
        if (numberOfBugs <= 0)
        {
            HUDManager.Instance.DisplayMessage("You don't have a device to bug this person.");
            return;
        }
        bugAttachment.AddBug(transform.position);
        ChangeNumBugs(-1);
    }

    public void AddItem(TakeableItem.ItemType itemType, int id)
    {
        if (itemType == TakeableItem.ItemType.bug)
        {
            ChangeNumBugs(1);
        }
        if (itemType == TakeableItem.ItemType.key)
        {
            AddKey(id);
        }
    }

    public void ChangeNumBugs(int changeAmount)
    {
        numberOfBugs = Mathf.Max(numberOfBugs + changeAmount, 0);
        HUDManager.Instance.SetNumberOfBugs(numberOfBugs);
    }

    public void AddKey(int keyId)
    {
        obtainedKeyIds.Add(keyId);
        HUDManager.Instance.SetObtainedKeys(obtainedKeyIds);
    }

    public bool HasKeyWithId(int value)
    {
        return obtainedKeyIds.Contains(value);
    }

    public void ProcessHideInput(InputAction.CallbackContext context)
    {

    }

    public void ProcessMenuButtonInput(InputAction.CallbackContext context)
    {

    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!displayInteractionSphereGizmo)
        {
            return;
        }
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Gizmos.DrawSphere(interactionPosition.transform.position, interactionRadius);
    }

#endif
}
