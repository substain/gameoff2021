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

    private Timer listenTimer;

    private AbstractActivity currentActivity;

    private AudioSource targetAudioSource = null;

    private float timeListened = 0; 

    void Start()
    {
        this.audioSource = GetComponentInParent<AudioSource>();
    }

    void Awake()
    {
        listenTimer = gameObject.AddComponent<Timer>();
        debugBug.SetActive(bugIsAttached);
    }

    public void SetCurrentActivity(AbstractActivity activity)
    {
        timeListened = 0;
        currentActivity = activity;


        if (targetAudioSource != null)
        {
            ConstraintManager.GameConstraint? constraint = activity.GetGameConstraint();

            if (constraint.HasValue && activity.GetNeededTimeToListen() < timeListened)
            {
                listenTimer.SetPaused(false);

                listenTimer.Init(currentActivity.GetNeededTimeToListen(), ReachedConstraintTime);
            }
            StartPlayingSoundAt(targetAudioSource, activity.GetAudioClip(), 0f, activity.IsContinuous());
        }
    }

    private void ReachedConstraintTime()
    {
        ConstraintManager.Instance.SetSatisfied(currentActivity.GetGameConstraint().Value);
    }

    public float GetCurrentAudioClipPos()
    {
        return listenTimer.GetTimePassed();
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
        ConstraintManager.GameConstraint? constraint = currentActivity.GetGameConstraint();

        if (constraint.HasValue && currentActivity.GetNeededTimeToListen() < timeListened)
        {
            listenTimer.SetPaused(false);
        }
        this.audioSource = source;

        StartPlayingSoundAt(source, currentActivity.GetAudioClip(), currentActivity.GetTimeProgress(), currentActivity.IsContinuous());

        currentActivity.GetAudioClip();
        this.audioSource.spatialBlend = 0.2f;
    }

    public void StopListening()
    {
        ConstraintManager.GameConstraint? constraint = currentActivity.GetGameConstraint();

        if (constraint.HasValue && currentActivity.GetNeededTimeToListen() < timeListened)
        {
            listenTimer.SetPaused(true);
        }
        this.audioSource.spatialBlend = 1;
        this.audioSource.Stop();
    }

    public void StartPlayingSoundAt(AudioSource source, AudioClip clip, float clipTimePosition, bool looping)
    {
        source.clip = clip;
        source.loop = looping;
        source.time = clipTimePosition > 0 ? clipTimePosition : 0f;
        source.Play();
    }
}