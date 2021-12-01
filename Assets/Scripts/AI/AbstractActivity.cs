
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractActivity : MonoBehaviour
{
    [Tooltip("The order of this activity. Will not be used if the actities are done in random order.")]
    [SerializeField]
    protected int order;

    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    protected bool contentLoopable;

    protected bool activityActive = false;

    protected GameObject controlledGameObject;

    private AudioSource audioSource;

    protected BugAttachment bugAttachment;

    [SerializeField]
    private string activityString;

    [SerializeField]
    private ConstraintManager.GameConstraint listenRewardConstraint = ConstraintManager.GameConstraint.none;

    [SerializeField]
    private ConstraintManager.GameConstraint activationConstraint = ConstraintManager.GameConstraint.none;

    [SerializeField]
    private ConstraintManager.GameConstraint deactivationConstraint = ConstraintManager.GameConstraint.none;
    
    private bool constraintsSatisfied = false;
    protected bool isPaused = false;

    public void Init(GameObject controlledObject)
    {
        controlledGameObject = controlledObject;
        audioSource = controlledGameObject.GetComponent<AudioSource>();
    }

    public ConstraintManager.GameConstraint? GetGameConstraint()
    {
        if(listenRewardConstraint != ConstraintManager.GameConstraint.none)
        {
            return listenRewardConstraint;
        }
        return null;
    }

    public abstract bool IsContinuous();

    public AudioClip GetAudioClip()
    {
        return audioClip;
    }

    public void SetBugAttachment(BugAttachment bugAttachment)
    {
        this.bugAttachment = bugAttachment;
    }

    public void StartActivity()
    {
        activityActive = true;
        DoStartActivity();
        if(audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = contentLoopable;
            audioSource.Play();
        }
    }

    protected abstract void DoStartActivity();

    public void StopActivity()
    {
        activityActive = false;
        if (audioClip != null)
        {
            audioSource.Stop();
        }
        DoStopActivity();
    }

    protected abstract void DoStopActivity();

    public bool CheckIfFinished()
    {
        if (isPaused)
        {
            return false;
        }
        return IsFinished();
    }

    protected abstract bool IsFinished();

    public int GetOrder()
    {
        return order;
    }

    public string GetActivityString()
    {
        return activityString;
    }

    public virtual void SetPaused(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public abstract float GetNeededTimeToListen();

    public virtual float GetFullDisplayTime()
    {
        return -1;
    }

    public virtual float GetTimeProgress()
    {
        return -1;
    }

    public void CheckActivationConstraints()
    {
        if (activationConstraint != ConstraintManager.GameConstraint.none)
        {
            constraintsSatisfied = ConstraintManager.Instance.IsSatisfied(activationConstraint);
        }
        else
        {
            constraintsSatisfied = true;
        }

        if (constraintsSatisfied && deactivationConstraint != ConstraintManager.GameConstraint.none)
        {
            constraintsSatisfied = !ConstraintManager.Instance.IsSatisfied(deactivationConstraint);
        }
    }

    public bool HasConstraintsSatisfied()
    {
        return constraintsSatisfied;
    }
}
