using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dialogue
{
    public const float INTERACTION_RANGE = 6.0f;

    private readonly DialogueHolder.DialogueKey key;
    private readonly bool isOneShot = true;
    private readonly bool isBlocking = false;
    private readonly List<ConstraintManager.GameConstraint> constraints = new List<ConstraintManager.GameConstraint>();
    private readonly List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private int lineIndex = 0;

    public Dialogue(DialogueHolder.DialogueKey key, List<DialogueLine> dialogueLines, bool isOneShot, bool isBlocking, List<ConstraintManager.GameConstraint> constraints)
    {
        this.key = key;
        this.isOneShot = isOneShot;
        this.isBlocking = isBlocking;
        this.constraints = constraints;
        this.dialogueLines = dialogueLines;
    }

    public void ReplaceSubjectPlaceholderWith(string name)
    {
        foreach(DialogueLine dl in dialogueLines)
        {
            dl.ReplaceSubjectPlaceholderWith(name);
        }
        dialogueLines.ForEach(dl => { dl.ReplaceSubjectPlaceholderWith(name); });
    }

    public DialogueLine GetNextLine()
    {
        DialogueLine line = GetLineWithIndex(lineIndex);
        lineIndex++;
        return line;
    }

    public DialogueLine GetLineWithIndex(int index)
    {
        if (index >= dialogueLines.Count)
        {
            return null;
        }
        return dialogueLines[index];
    }
    
    public void SetFinished()
    {
        ConstraintManager.GameConstraint? constraint = ToGameConstraint(key);
        if (constraint.HasValue)
        {
            string rewardString = ConstraintManager.ConstraintToRewardString(constraint.Value);
            if(rewardString.Length > 0)
            {
                HUDManager.HUDInstance.DisplayMessage(rewardString);
                ConstraintManager.Instance.PlayRewardSound();
            }
            ConstraintManager.Instance.SetSatisfied(constraint.Value);
        }
        if (isOneShot)
        {
            DialogueManager.Instance.SetFinished(key);
        }
        Reset();
    }

    public void Reset()
    {
        lineIndex = 0;
    }

    public bool IsAvailable()
    {
        if (lineIndex > 0)
        {
            return false;
        }
        bool constraintsSatisfied = ConstraintManager.Instance.AllConstraintsSatisfied(constraints);

        return constraintsSatisfied && !DialogueManager.Instance.IsFinished(key);
    }

    public bool IsOneShot()
    {
        return isOneShot;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public DialogueHolder.DialogueKey GetKey()
    {
        return key;
    }

    public string GetFullText()
    {
        return dialogueLines.Select(dl => GetReplacedText(dl)).Aggregate((dl1, dl2) => dl1 + "\n" + dl2);
    }

    private static string GetReplacedText(DialogueLine dialogueLine)
    {
        return InputKeyHelper.Instance.ReplacePlaceholdersWithCurrentKeys(dialogueLine.content);
    }

    public static ConstraintManager.GameConstraint? ToGameConstraint(DialogueHolder.DialogueKey dialogueKey)
    {
        switch (dialogueKey){
            case DialogueHolder.DialogueKey.shipReachedMono:
            case DialogueHolder.DialogueKey.weaponFound:
            {
                    return ConstraintManager.GameConstraint.loadNextLevel;
                }
            case DialogueHolder.DialogueKey.firstMeetScarlet:
                {
                    return ConstraintManager.GameConstraint.gotKey2;
                }
        }
        return null;
    }
}