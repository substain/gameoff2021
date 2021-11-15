
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractActivity : MonoBehaviour
{
    [Tooltip("The order of this activity. Will not be used if the actities are done in random order.")]
    [SerializeField]
    protected int order;

    [Tooltip("How alert the npc is doing this activity (0-1)")]
    [SerializeField]
    protected float baseAlert = 0.5f;

    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    private bool audioClipLoopable;

    protected bool activityActive = false;

    protected GameObject controlledGameObject;

    private AudioSource audioSource;
    public void Init(GameObject controlledObject)
    {
        controlledGameObject = controlledObject;
        audioSource = controlledGameObject.GetComponent<AudioSource>();
    }

    public void StartActivity()
    {
        activityActive = true;
        DoStartActivity();
        if(audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = audioClipLoopable;
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
    public abstract bool IsFinished();

    public int GetOrder()
    {
        return order;
    }

    public abstract void SetPaused(bool isPaused);
}
