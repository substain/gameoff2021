using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private LayerMask layerToCheckFor;
    private PlayerMovement playerMovement;

    private List<BugAttachment> attachedBugs = new List<BugAttachment>();

    private int listenBugIndex = -1;

    private IInteractable currentInteractable;

    private bool isInBlockingDialogue;
    private bool isInMenu = false;

    void Awake()
    {
        if(interactionPosition == null)
        {
            Debug.LogWarning("no interactionPosition assigned!");
        }

        layerToCheckFor = LayerMask.GetMask(INTERACTION_LMASK_NAME);
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        HUDManager.Instance.SetNumberOfBugs(numberOfBugs);
        HUDManager.Instance.SetObtainedKeys(obtainedKeyIds);
        HUDManager.Instance.SetCurrentActiveBugId(listenBugIndex);
        InputKeyHelper.Instance.SetPlayerInput(GetComponent<PlayerInput>());
        InvokeRepeating("CheckForInteractables", 0.1f, 0.1f);
        GameManager.Instance.SetPlayer(this);
    }

    private void CheckForInteractables()
    {
        bool hasActiveDialogue = CheckForActiveDialogue();
        if (hasActiveDialogue)
        {
            return;
        }

        IInteractable previousInteractable = currentInteractable;

        List<IInteractable> foundInteractables = FindInteractablesInRange();
        List<IInteractable> orderedInteractables = SortAndFilterInteractables(foundInteractables);

        if(orderedInteractables.Count > 0)
        {
            currentInteractable = orderedInteractables[0];
            string keyname = InputKeyHelper.Instance.GetNameForKey(InputKeyHelper.ControlType.Interact);
            HUDManager.Instance.UpdateActionHintText("Press "+ keyname + " to " + currentInteractable.GetInteractionTypeString());
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

    private List<IInteractable> FindInteractablesInRange()
    {
        List<IInteractable> allInteractables = new List<IInteractable>();
        Collider[] matchingColliders = Physics.OverlapSphere(interactionPosition.transform.position, interactionRadius, layerToCheckFor);

        foreach (Collider col in matchingColliders)
        {
            IInteractable[] foundInteractables = col.gameObject.GetComponents<IInteractable>();
            if (foundInteractables != null)
            {
                allInteractables.AddRange(foundInteractables);
            }
        }
        return allInteractables;
    }

    private bool CheckForActiveDialogue()
    {
        if (currentInteractable != null && IsDialogueHolder(currentInteractable))
        {
            //has active dialogue
            DialogueHolder dialogueHolder = (DialogueHolder)currentInteractable;
            if (dialogueHolder.HasActiveDialogue())
            {
                if (((DialogueHolder)currentInteractable).IsInRange(this.transform))
                {
                    return true;
                }
                else
                {
                    //player went out of range of the dialogue -> reset the dialogue
                    dialogueHolder.CancelDialogue();
                }
            }
        }
        return false;
    }

    private List<IInteractable> SortAndFilterInteractables(List<IInteractable> unorderedInteractables)
    {
        //TODO: prio bugs, if enemies are not looking in your direction
        //Then: prio dialogues
        //Then: bugs, when they are looking in your direction?
        unorderedInteractables.RemoveAll(ia => IsDialogueHolder(ia) && !((DialogueHolder)ia).HasValidNewDialogue());

        //prioritize dialogues
        List<IInteractable> sortedInteractables = unorderedInteractables.OrderByDescending(ia => IsDialogueHolder(ia)).ToList();

        //remove dialogues without active dialog option

        return sortedInteractables;
    }

    public void ProcessInteractInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (isInMenu)
        {
            HUDManager.Instance.IngameMenuUseSelected();
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

        for(int i = 0; i < attachedBugs.Count; i++)
        {
            if (bugAttachment == attachedBugs[i])
            {
                if (listenBugIndex > i)
                {
                    listenBugIndex--;
                }
                attachedBugs.RemoveAt(i);
                break;
            }
        }
    }

    public void PutBugOn(BugAttachment bugAttachment)
    {
        if (numberOfBugs <= 0)
        {
            HUDManager.Instance.DisplayMessage("You don't have a device to bug this person.");
            return;
        }
        attachedBugs.Add(bugAttachment);
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

    public void ProcessListenBugInput(InputAction.CallbackContext context)
    {
        if (isInBlockingDialogue || isInMenu)
        {
            return;
        }
        //Stop previous bug
        if (listenBugIndex > -1)
        {
            attachedBugs[listenBugIndex].StopListening();
        }

        //Cycle through attached bugs, stop listening if the end is reached (listenBugIndex == -1)
        if (attachedBugs.Count > 0)
        {
            listenBugIndex++;
            if(listenBugIndex >= attachedBugs.Count)
            {
                listenBugIndex = -1;
            }
        }
    
        //start current bug
        if(listenBugIndex > -1)
        {
            //attachedBugs[listenBugIndex].StartListening();
        }
        HUDManager.Instance.SetCurrentActiveBugId(listenBugIndex);
    }

    public void ProcessHideInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (isInMenu)
        {
            HUDManager.Instance.IngameMenuUseBack();
        }
        if (isInBlockingDialogue)
        {
            return;
        }
        if (currentInteractable != null && IsDialogueHolder(currentInteractable))
        {
            DialogueHolder dialogueHolder = (DialogueHolder)currentInteractable;
            dialogueHolder.CancelDialogue();
            return;
        }
    }
    public void SetBlockingDialogueActive(bool isBlocked)
    {
        this.isInBlockingDialogue = isBlocked;
        playerMovement.SetBlockingDialogueActive(isBlocked);
    }

    public void SetMenuActive(bool isInMenu)
    {
        this.isInMenu = isInMenu;
        playerMovement.SetMenuActive(isInMenu);
    }

    public void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
    }

    public void ProcessMenuButtonInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (isInMenu)
        {
            HUDManager.Instance.IngameMenuUseBack();
            return;
        }
        GameManager.Instance.StartPauseMenu();
    }

    private static bool IsDialogueHolder(IInteractable interactable)
    {
        return (interactable.GetType() == typeof(DialogueHolder) || interactable.GetType().IsSubclassOf(typeof(DialogueHolder)));
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