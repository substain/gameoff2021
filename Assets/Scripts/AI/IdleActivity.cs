using System.Collections.Generic;
using UnityEngine;
public class IdleActivity : AbstractActivity
{
    private const float FRACTION_NEEDED_TO_LISTEN = 0.6f;

    [SerializeField]
    private float idleTime;

    private Timer timer;

    private float currentTime;
    void Awake()
    {
        timer = GetComponent<Timer>();
    }

    protected override void DoStartActivity()
    {
        timer.Init(idleTime);
    }

    protected override void DoStopActivity()
    {
        timer.Finish();
    }

    public override bool IsContinuous()
    {
        return contentLoopable;
    }

    protected override bool IsFinished()
    {
        return !timer.IsRunning();
    }

    public override void SetPaused(bool isPaused)
    {
        timer.SetPaused(isPaused);
        base.SetPaused(isPaused);
    }

    public override float GetNeededTimeToListen()
    {
        return idleTime * FRACTION_NEEDED_TO_LISTEN;
    }

    public override float GetFullDisplayTime()
    {
        return timer.GetFullDuration();
    }

    public override float GetTimeProgress()
    {
        return timer.GetTimePassed();
    }
}