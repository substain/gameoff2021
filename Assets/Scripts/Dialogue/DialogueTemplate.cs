using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueTemplate
{
    public readonly struct DialogueTemplateLine
    {
        public DialogueTemplateLine(string subject, string content)
        {
            this.content = content;
            this.subject = subject;
        }

        private string subject { get; }
        private string content { get; }

        public DialogueLine GetDialogueLine()
        {
            return new DialogueLine(subject, content);
        }
    }

    private List<DialogueTemplateLine> dialogueTemplateLines = new List<DialogueTemplateLine>();
    private DialogueHolder.DialogueKey key;
    private bool isOneShot = true;
    private List<ConstraintManager.GameConstraint> constraints = new List<ConstraintManager.GameConstraint>();


    public Dialogue ToDialogue()
    {
        List<DialogueLine> dialogueLines = dialogueTemplateLines.Select(dtl => dtl.GetDialogueLine()).ToList();
        return new Dialogue(key, isOneShot, new List<ConstraintManager.GameConstraint>(constraints), dialogueLines);
    }

    public void AddLine(string subject, string content)
    {
        dialogueTemplateLines.Add(new DialogueTemplateLine(subject, content));
    }

    public void AddLine(string line)
    {
        dialogueTemplateLines.Add(new DialogueTemplateLine("", line));
    }

    public void SetKey(DialogueHolder.DialogueKey key)
    {
        this.key = key;
    }

    public void SetIsOneShot(bool isOneShot)
    {
        this.isOneShot = isOneShot;
    }

    public void AddConstraint(ConstraintManager.GameConstraint constraint)
    {
        constraints.Add(constraint);
    }

}