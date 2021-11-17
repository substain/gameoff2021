using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Dialogue
{
    public const float INTERACTION_RANGE = 6.0f;

    private DialogueHolder.DialogueKey key;
    private bool isOneShot = true;
    private List<ConstraintManager.GameConstraint> constraints = new List<ConstraintManager.GameConstraint>();
    private List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private bool wasUsed = false;

    //private Transform dialogueTarget;
    private int lineIndex = 0;

    public Dialogue(DialogueHolder.DialogueKey key, bool isOneShot, List<ConstraintManager.GameConstraint> constraints, List<DialogueLine> dialogueLines)
    {
        this.key = key;
        this.isOneShot = isOneShot;
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

    /*
    public void SetDialogueTarget(Transform dialogueTarget)
    {
        this.dialogueTarget = dialogueTarget;
    }

    public Transform GetDialogueTarget()
    {
        return dialogueTarget;
    } */

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
        if (isOneShot)
        {
            wasUsed = true;
        }
        Reset();
    }

    public void Reset()
    {
        lineIndex = 0;
    }

    public bool IsAvailable()
    {
        bool constraintsSatisfied = ConstraintManager.Instance.AllConstraintsSatisfied(constraints);
        return constraintsSatisfied && !wasUsed;
    }

    public bool IsOneShot()
    {
        return isOneShot;
    }
}
