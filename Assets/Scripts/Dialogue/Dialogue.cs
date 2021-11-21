using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Dialogue
{
    public const float INTERACTION_RANGE = 6.0f;

    private readonly DialogueHolder.DialogueKey key;
    private readonly bool isOneShot = true;
    private readonly bool isBlocking = false;
    private readonly List<ConstraintManager.GameConstraint> constraints = new List<ConstraintManager.GameConstraint>();
    private readonly List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private bool wasCompleted = false;
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
        if (isOneShot)
        {
            wasCompleted = true;
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
        return constraintsSatisfied && !wasCompleted;
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
}