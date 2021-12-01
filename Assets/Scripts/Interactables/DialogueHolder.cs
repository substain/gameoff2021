using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHolder : MonoBehaviour, IInteractable
{
    public enum DialogueKey
    {
        anyoneSpeechless, anyoneBusy,                           //everywhere
        introMonologue,
        useTutorialMono, dashTutorialMono,                      //only tutorialDialogues.txt
        avoidTutorialMono, bugTutorialMono, //only tutorialDialogues.txt
        shipReachedMono, cantUseStairs, weaponDestroyed, weaponLeft,
        firstMeetScarlet
    }

    [SerializeField]
    private string subjectName = "";

    [SerializeField]
    private string interactionString = "talk";

    [SerializeField]
    private List<DialogueKey> dialogueKeys;

    protected List<Dialogue> oneShotDialogues = new List<Dialogue>();
    protected List<Dialogue> repeatableDialogues = new List<Dialogue>();

    protected List<Dialogue> availableOneshots = new List<Dialogue>();
    protected List<Dialogue> availableRepeatables = new List<Dialogue>();

    protected Dialogue currentDialogue = null;


    void Start()
    {
        CollectDialogues();
        ConstraintManager.OnChangeConstraints += CheckAvailableDialogues;
        CheckAvailableDialogues();
    }

    protected void CollectDialogues()
    {
        foreach (DialogueKey dk in dialogueKeys)
        {
            Dialogue dialogue = DialogueManager.Instance.GetDialogueTemplate(dk).ToDialogue();
            if (dialogue != null)
            {
                if (subjectName.Length > 0)
                {
                    dialogue.ReplaceSubjectPlaceholderWith(subjectName);
                }

                //dialogue.SetDialogueTarget(this.transform);

                if (dialogue.IsOneShot())
                {
                    oneShotDialogues.Add(dialogue);
                }
                else
                {
                    repeatableDialogues.Add(dialogue);
                }
            }
        }
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        if (currentDialogue == null)
        {
            currentDialogue = GetNextDialogue();
        }

        if (currentDialogue == null)
        {
            Debug.LogWarning("Trying to interact with a dialogue that has no next dialogue!");
            return;
        }
        if (currentDialogue.IsBlocking())
        {
            interactingPlayer.SetBlockingDialogueActive(true);
        }
        ProgressDialogue(interactingPlayer);
    }

    public bool HasActiveDialogue()
    {
        return currentDialogue != null;
    }

    /// <summary>
    /// Progresses the current dialogue. Returns true, if the dialogue is finished after this.
    /// </summary>
    public bool ProgressDialogue(PlayerInteraction interactingPlayer)
    {
        if (HUDManager.Instance.IsTypingDialogue())
        {
            HUDManager.Instance.FinishTypingDialogue();
            return false;
        }
        DialogueLine nextLine = currentDialogue.GetNextLine();
        if (nextLine == null)
        {
            FinishDialogue(interactingPlayer);
            return true;
        }
        HUDManager.Instance.ShowDialogue(nextLine, interactingPlayer.transform, transform);

        return false;
    }

    protected virtual void FinishDialogue(PlayerInteraction interactingPlayer)
    {
        currentDialogue.SetFinished();
        if (currentDialogue.IsOneShot())
        {
            interactingPlayer.SetBlockingDialogueActive(false);
            oneShotDialogues.RemoveAll(d => d == currentDialogue);
            availableOneshots.RemoveAll(d => d == currentDialogue);
        }
        currentDialogue = null;

        HUDManager.Instance.CloseDialogue();
    }

    public virtual bool HasValidNewDialogue()
    {
        return GetNextDialogue() != null;
    }

    private Dialogue GetNextDialogue()
    {
        if(availableOneshots.Count > 0)
        {
            return availableOneshots[0];
        }

        if (availableRepeatables.Count > 0)
        {        
            //repeatable dialogs can be used in random order
            Util.ShuffleList(repeatableDialogues);

            return repeatableDialogues[0];
        }
        return null;
    }


    protected virtual void CheckAvailableDialogues()
    {
        availableOneshots.Clear();
        availableRepeatables.Clear();
        //prefer oneshot dialogues because they are probably for the story
        foreach (Dialogue dialogue in oneShotDialogues)
        {
            if (dialogue.IsAvailable())
            {
                availableOneshots.Add(dialogue);
            }
        }

        foreach (Dialogue dialogue in repeatableDialogues)
        {
            if (dialogue.IsAvailable())
            {
                availableRepeatables.Add(dialogue);
            }
        }
    }

    public virtual bool IsInRange(Transform playerTransform)
    {
        //Debug.Log("isinrange:" + (Vector3.Distance(this.transform.position, playerTransform.position) < Dialogue.INTERACTION_RANGE));
        return Vector3.Distance(this.transform.position, playerTransform.position) < Dialogue.INTERACTION_RANGE;
    }

    public virtual void CancelDialogue()
    {
        HUDManager.Instance.CloseDialogue();
        currentDialogue?.Reset();
        currentDialogue = null;
    }

    public virtual string GetInteractionTypeString()
    {
        return interactionString;
    }

    void OnDestroy()
    {
        ConstraintManager.OnChangeConstraints -= CheckAvailableDialogues;
    }
}