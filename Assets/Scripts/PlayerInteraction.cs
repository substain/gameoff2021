using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static HUDBugDisplay;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const string INTERACTION_LMASK_NAME = "Interactable";
    private int NUM_BUGS_POSSIBLE = 3;

    [SerializeField]
    private GameObject interactionPosition;

    [SerializeField]
    private float interactionRadius = 2f;

    [SerializeField]
    private int numberOfBugs = 0;

    [SerializeField]
    private bool displayInteractionSphereGizmo = true;

    [SerializeField]
    private HashSet<int> obtainedKeyIds;

    private LayerMask layerToCheckFor;
    private PlayerMovement playerMovement;

    private List<BugAttachment> attachedBugs = new List<BugAttachment>();

    private int listenBugIndex = -1;

    private IInteractable currentInteractable;

    private bool isInBlockingDialogue;
    private bool isInMenu = false;
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {

        if (interactionPosition == null)
        {
            Debug.LogWarning("no interactionPosition assigned!");
        }

        layerToCheckFor = LayerMask.GetMask(INTERACTION_LMASK_NAME);
        playerMovement = GetComponent<PlayerMovement>();
        this.animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        obtainedKeyIds = new HashSet<int>(CheckpointManager.GetObtainedKeyIds());
    }

    void Start()
    {
        UpdateBugStates();
        HUDManager.HUDInstance.SetObtainedKeys(obtainedKeyIds);
        InputKeyHelper.Instance.SetPlayerInput(GetComponent<PlayerInput>());
        InvokeRepeating("CheckForInteractables", 0.1f, 0.1f);
        GameManager.GameInstance.SetPlayer(this);
        ConstraintManager.OnChangeConstraints += UpdateAvailableKeys;
    }

    private void UpdateAvailableKeys()
    {
        HashSet<int> availableKeys = ConstraintManager.Instance.GetKeyConstraints()
            .Select(gc => Convert.ToInt32(gc.ToString().Replace(ConstraintManager.KEY_PREFIX, ""))).ToHashSet();

        obtainedKeyIds.UnionWith(availableKeys);
        HUDManager.HUDInstance.SetObtainedKeys(obtainedKeyIds);
    }

    public HashSet<int> GetObtainedKeyIds()
    {
        return obtainedKeyIds;
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
            HUDManager.HUDInstance.UpdateActionHintText("Press "+ keyname + " to " + currentInteractable.GetInteractionTypeString());
        }
        else
        {
            currentInteractable = null;
            if(previousInteractable != null)
            {
                HUDManager.HUDInstance.UpdateActionHintText("");
            }
        }
    }

    internal bool JustAteCheese()
    {
        throw new NotImplementedException();
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
        List<IInteractable> sortedInteractables = unorderedInteractables.OrderByDescending(ia => InteractableToOrder(ia)).ToList();

        //remove dialogues without active dialog option

        return sortedInteractables;
    }

    private int InteractableToOrder(IInteractable interactable)
    {
        if (interactable.GetType() == typeof(WeaponChangerInteractable))
        {
            return 5;
        }

        if (interactable.GetType() == typeof(CheeseStickInteractable))
        {
            return 4;
        }

        if (interactable.GetType() == typeof(TakeableItem))
        {
            return 3;
        }

        if (IsDialogueHolder(interactable))
        {
            return 2;
        }

        if (interactable.GetType() == typeof(BugAttachment))
        {
            return 1;
        }

        return -1;
    }

    public void ProcessInteractInput(InputAction.CallbackContext context)
    {
        if (isInMenu || !context.performed)
        {
            return;
        }
        if (currentInteractable == null)
        {
            return;
        }

        currentInteractable.Interact(this);
        if (!IsDialogueHolder(currentInteractable))
        {
            animator.SetBool("use", true);
        }
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
            HUDManager.HUDInstance.DisplayMessage("You don't have a device to bug this person.");
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
        UpdateBugStates();
    }

    public void AddKey(int keyId)
    {
        obtainedKeyIds.Add(keyId);
        HUDManager.HUDInstance.SetObtainedKeys(obtainedKeyIds);
    }

    public bool HasKeyWithId(int value)
    {
        return obtainedKeyIds.Contains(value);
    }

    public void ProcessListenBugInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
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
            attachedBugs[listenBugIndex].StartListening(audioSource);
        }
        UpdateBugStates();
    }

    public void ProcessHideInput(InputAction.CallbackContext context)
    {
        if (isInMenu || !context.performed)
        {
            return;
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
        if (isInMenu || !context.performed)
        {
            return;
        }
        GameManager.GameInstance.StartPauseMenu();
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    private static bool IsDialogueHolder(IInteractable interactable)
    {
        return (interactable.GetType() == typeof(DialogueHolder) || interactable.GetType().IsSubclassOf(typeof(DialogueHolder)));
    }

    private void OnDestroy()
    {
        ConstraintManager.OnChangeConstraints -= UpdateAvailableKeys;
    }

    private void UpdateBugStates()
    {
        BugState[] bugStates = new BugState[NUM_BUGS_POSSIBLE];
        for (int i = 0; i < NUM_BUGS_POSSIBLE; i++)
        {
            if (i == listenBugIndex)
            {
                bugStates[i] = BugState.bugSending;
                continue;
            }
            if (i < attachedBugs.Count)
            {
                bugStates[i] = BugState.bugPlaced;
                continue;
            }
            if (i < numberOfBugs + attachedBugs.Count)
            {
                bugStates[i] = BugState.bugAvailable;
                continue;
            }
            bugStates[i] = BugState.bugEmpty;
        }


        HUDManager.HUDInstance.SetBugStates(bugStates);
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