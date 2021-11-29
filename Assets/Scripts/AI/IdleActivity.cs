using System.Collections.Generic;
using UnityEngine;
public class IdleActivity : AbstractActivity
{
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
        Debug.Log("starting movement activity");
        timer.Init(idleTime);
    }
    protected override void DoStopActivity()
    {
        timer.Finish();
    }

    public override bool IsContinuous()
    {
        return false;
    }

    public override bool IsFinished()
    {
        return !timer.IsRunning();
    }

    public override void SetPaused(bool isPaused)
    {
        timer.SetPaused(isPaused);
    }
}