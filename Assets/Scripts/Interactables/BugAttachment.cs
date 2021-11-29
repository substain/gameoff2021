using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugAttachment : MonoBehaviour, IInteractable
{
    private AudioSource audioSource;

    [SerializeField]
    private GameObject debugBug;

    private bool bugIsAttached = false;

    private Timer activityTimer;

    private AbstractActivity currentActivity;

    private AudioSource targetAudioSource = null;

    void Start()
    {
        this.audioSource = GetComponentInParent<AudioSource>();
    }

    void Awake()
    {
        activityTimer = gameObject.AddComponent<Timer>();
        debugBug.SetActive(bugIsAttached);
    }

    public void SetCurrentActivity(AbstractActivity activity)
    {
        currentActivity = activity;
        if (targetAudioSource != null)
        {
            //StartPlayingSoundAt(targetAudioSource, activity.GetAudioClip(), 0f, activity.IsContinuous());
        }
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
    }

    public void AddBug(Vector3 fromPosition)
    {
        bugIsAttached = true;
        debugBug.SetActive(true);
        float xPos = fromPosition.x < transform.position.x ? -0.65f : 0.65f;

        debugBug.transform.position = transform.root.position + new Vector3(xPos, 0, 0);
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

    public void StartListening(AudioSource source)
    {
        this.audioSource = source;
        currentActivity.GetAudioClip();
        this.audioSource.spatialBlend = 0.2f;
    }

    public void StopListening()
    {
        this.audioSource.spatialBlend = 1;
    }

    public void StartPlayingSoundAt(AudioSource source, AudioClip clip, float clipTimePosition, bool looping)
    {
        source.clip = clip;
        source.loop = looping;
        source.time = clipTimePosition;
        source.Play();
    }

}