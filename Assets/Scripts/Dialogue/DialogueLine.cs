using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLine
{
    public string subject { get; set; }
    public string content { get; }

    public DialogueLine(string subject, string content)
    {
        this.content = content;
        this.subject = subject;
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
}
