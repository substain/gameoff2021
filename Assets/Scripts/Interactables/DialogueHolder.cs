using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueHolder : MonoBehaviour, IInteractable
{
    public enum DialogueKey
    {
        testDialogue1, testDialogue2, introMonologue, anyoneSpeechless, anyoneBusy
    }

    [SerializeField]
    private string subjectName = "";

    [SerializeField]
    private List<DialogueKey> dialogueKeys;

    private List<Dialogue> oneShotDialogues = new List<Dialogue>();
    private List<Dialogue> repeatableDialogues = new List<Dialogue>();


    private Dialogue currentDialogue = null;

    void Start()
    {
        foreach(DialogueKey dk in dialogueKeys)
        {
            Dialogue dialogue = DialogueManager.Instance.GetDialogueTemplate(dk).ToDialogue();
            if(dialogue != null)
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
        ProgressDialogue(interactingPlayer.transform);
    }

    public bool HasActiveDialogue()
    {
        return currentDialogue != null;
    }

    /// <summary>
    /// Progresses the current dialogue. Returns true, if the dialogue is finished after this.
    /// </summary>
    public bool ProgressDialogue(Transform playerTransform)
    {
        DialogueLine nextLine = currentDialogue.GetNextLine();
        if (nextLine == null)
        {
            currentDialogue.SetFinished();
            currentDialogue = null;
            HUDManager.Instance.CloseDialogue();
            return true;
        }
        HUDManager.Instance.ShowDialog(nextLine, playerTransform, transform);
        return false;
    }

    public bool HasValidNewDialogue()
    {
        bool hasNextDialogue = GetNextDialogue() != null;
        return hasNextDialogue;
    }

    private Dialogue GetNextDialogue()
    {
        //prefer oneshot dialogues because they are probably for the story
        foreach (Dialogue dialogue in oneShotDialogues)
        {
            if (dialogue.IsAvailable())
            {
                return dialogue;
            }
        }

        //repeatable dialogs can be used in random order
        Util.ShuffleList(repeatableDialogues);

        foreach (Dialogue dialogue in repeatableDialogues)
        {
            if (dialogue.IsAvailable())
            {
                return dialogue;
            }
        }
        return null;
    }

    public bool IsInRange(Transform playerTransform)
    {
        //Debug.Log("isinrange:" + (Vector3.Distance(this.transform.position, playerTransform.position) < Dialogue.INTERACTION_RANGE));
        return Vector3.Distance(this.transform.position, playerTransform.position) < Dialogue.INTERACTION_RANGE;
    }

    public void CancelDialoge()
    {
        HUDManager.Instance.CloseDialogue();
        currentDialogue?.Reset();
        currentDialogue = null;
    }

    public string GetInteractionTypeString()
    {
        return "talk";
    }
}