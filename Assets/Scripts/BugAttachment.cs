using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAttachment : MonoBehaviour, IInteractable
{
    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> activitySoundClips;

    [SerializeField]
    private GameObject debugBug;

    private bool bugIsAttached = false;

    private Timer activityTimer;

    private int activityIndex = 0;

    void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        activityTimer = gameObject.AddComponent<Timer>();
        debugBug.SetActive(bugIsAttached);
    }

    internal void SetId(int v)
    {
        throw new NotImplementedException();
    }

    public void StartActivity(int activityIndex, float activityDuration)
    {
        this.activityIndex = activityIndex;
        activityTimer.Init(activityDuration);
    }

    public AudioClip GetCurrentAudioClip()
    {
        return activitySoundClips[activityIndex];
    }

    public float GetCurrentAudioClipPos()
    {
        return activityTimer.GetTimePassed();
    }

    public bool HasBugAttached()
    {
        return bugIsAttached;
    }

    public void RemoveBug()
    {
        bugIsAttached = false;
        debugBug.SetActive(false);
        this.audioSource.spatialBlend = 1;
    }

    public void AddBug(Vector3 fromPosition)
    {
        bugIsAttached = true;
        debugBug.SetActive(true);
        float xPos = fromPosition.x < transform.position.x ? -0.65f : 0.65f;

        debugBug.transform.position = transform.root.position + new Vector3(xPos, 0, 0);
        this.audioSource.spatialBlend = 0.2f;
    }

    public void Interact(PlayerInteraction interactingPlayer)
    {
        //go the extra tour around the player 
        if (bugIsAttached)
        {
            interactingPlayer.RemoveBugFrom(this);
        }
        else
        {
            interactingPlayer.PutBugOn(this);
        }
    }

    public string GetInteractionTypeString()
    {
        if (bugIsAttached)
        {
            return "detach the bug";
        }
        else
        {
            return "attach a bug";
        }
    }
}