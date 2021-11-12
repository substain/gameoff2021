using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BugListeningHandler: MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    private int currentBugChannel;

    private List<BugAttachment> attachedBugs;

    public void AddToList(BugAttachment bug)
    {
        attachedBugs.Add(bug);
        //bug.SetId(attachedBugs.Count - 1);
    }

    public void RemoveFromList(int idToRemove)
    {
        if(idToRemove == currentBugChannel)
        {
            //the removed device is the one that is currently active
            audioSource.Stop();
        }
        if (currentBugChannel >= idToRemove)
        {
            currentBugChannel--;
        }

        attachedBugs.RemoveAt(idToRemove);
    }

    public void StartListeningToChannel(int channel)
    {
        SwitchToChannel(channel);
        StartListening();
    }

    public void SwitchToChannel(int channel)
    {
        currentBugChannel = Mathf.Clamp(channel, 0, attachedBugs.Count);
    }

    public void StartListening()
    {
        audioSource.Play();
    }
}
