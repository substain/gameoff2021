using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLine
{
    public string subject { get; set; }
    public string content { get; }

    public List<ConstraintManager.Choice> choices;

    public DialogueLine(string subject, string content)
    {
        this.content = content;
        this.subject = subject;
        this.choices = new List<ConstraintManager.Choice>();
    }

    public void AddChoice(ConstraintManager.Choice choice)
    {
        choices.Add(choice);
    }

    public string GetMergedLine()
    {
        if (subject == "")
        {
            return content;
        }
        return subject + ": " + content;
    }

    public void ReplaceSubjectPlaceholderWith(string name)
    {
        subject = subject.Replace(DialogueManager.SUBJECT_PLACEHOLDER, name);
    }

    public List<ConstraintManager.Choice> GetChoices()
    {
        return choices;
    }
}
