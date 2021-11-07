using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Timer : MonoBehaviour
{
    private float timePassed = 0;
    private float timeToWait = float.PositiveInfinity;
    private Action onTargetReached;
    private bool timerFinished = true;
    private bool timerChecked = false;
    private bool paused = false;

    void Update()
    {
        if (!IsRunning())
        {
            return;
        }

        timePassed += Time.deltaTime;

        if (timePassed >= timeToWait)
        {
            Finish();
        }
    }

    /// <summary>
    /// Initializes the timer with the given duration. 
    /// If a finalAction is provided, this will be executed once the given time has passed. 
    /// If doReset is true, the timer will be reset when this method is called.
    /// </summary>
    public void Init(float durationInSeconds, Action finalAction = null, bool doReset = true)
    {
        if (doReset)
        {
            ResetTimer();
        }
        timeToWait = durationInSeconds;
        onTargetReached = finalAction;
    }

    public void Finish()
    {
        timerFinished = true;
        onTargetReached?.Invoke();
    }

    public void Stop()
    {
        timerFinished = true;
    }

    public void SetPaused(bool pausedNew = true)
    {
        paused = pausedNew;
    }

    /// <summary>
    /// Resets the timer and starts counting again (if it is not paused).
    /// </summary>
    public void ResetTimer()
    {
        timePassed = 0;
        timerFinished = false;
        timerChecked = false;
    }

    public float GetTimePassed()
    {
        return timePassed;
    }

    public float GetRemainingTime()
    {
        return timeToWait - timePassed;
    }

    public float GetRelativeProgress()
    {
        return timePassed / timeToWait;
    }

    /// <summary>
    /// Checks if the timer finished. Will return false if the timer was reset.
    /// </summary>
    public bool IsFinished()
    {
        return timerFinished;
    }

    public bool IsRunning()
    {
        return !(timerFinished || paused);
    }

    public bool WasStarted()
    {
        return timePassed > 0;
    }

    /// <summary>
    /// Check if the timer finished and onceIsFinished was not checked before. Will return false if onceIsFinished was called before or the timer was reset.
    /// </summary>
    public bool OnceIsFinished()
    {
        if (timerFinished && !timerChecked)
        {
            timerChecked = true;
            return true;
        }

        return false;
    }

    public void SetTimePassed(float timePassed)
    {
        this.timePassed = timePassed;
    }
}