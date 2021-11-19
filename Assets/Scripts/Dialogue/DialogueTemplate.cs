using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueTemplate
{
    private List<DialogueLine> dialogueLines = new List<DialogueLine>();
    private DialogueHolder.DialogueKey key;
    private bool isOneShot = true;
    private bool isBlocking = false;
    private List<ConstraintManager.GameConstraint> constraints = new List<ConstraintManager.GameConstraint>();

    public Dialogue ToDialogue()
    {
        return new Dialogue(key, dialogueLines, isOneShot, isBlocking, new List<ConstraintManager.GameConstraint>(constraints));
    }

    public void AddLine(string subject, string content)
    {
        dialogueLines.Add(new DialogueLine(subject, content));
    }

    public void AddLine(string line)
    {
        dialogueLines.Add(new DialogueLine("", line));
    }

    public void AddChoiceToLastLine(ConstraintManager.Choice choice)
    {
        dialogueLines[dialogueLines.Count - 1].AddChoice(choice);
    }

    public void SetKey(DialogueHolder.DialogueKey key)
    {
        this.key = key;
    }

    public void SetIsOneShot(bool isOneShot)
    {
        this.isOneShot = isOneShot;
    }

    public void SetIsBlocking(bool isBlocking)
    {
        this.isBlocking = isBlocking;
    }

    public void AddConstraint(ConstraintManager.GameConstraint constraint)
    {
        constraints.Add(constraint);
    }

}