using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousActivity : AbstractActivity
{
    [SerializeField]
    private float changeAngleTime = 1f;

    [Range(0, 180)]
    [SerializeField]
    private float checkRadius = 60;

    [SerializeField]
    private float checkDuration = 2.5f;

    private float timePassed = 0;

    private NPCMovement npcMovement;
    private Timer timer;

    private Vector3 targetDir;

    private float previousYAngle;
    private float nextYAngle;

    private bool isActive = false;

    void Start()
    {
        this.order = -1; //not needed

        changeAngleTime = changeAngleTime * 1/SettingsManager.GetDifficultyModifier();

        timer = GetComponent<Timer>();
        npcMovement = controlledGameObject.GetComponent<NPCMovement>();
    }

    void Update()
    {
        if (isActive)
        {
            float yAngle = Mathf.LerpAngle(previousYAngle, nextYAngle, timer.GetRelativeProgress());
            Vector3 targetDirection = Quaternion.Euler(0, yAngle, 0) * Vector3.forward;
            npcMovement.SetLookDirection(targetDirection);
        }
    }

    public override float GetNeededTimeToListen()
    {
        return checkDuration;
    }

    public override bool IsContinuous()
    {
        return false;
    }

    public void SetTarget(Vector3 targetPos)
    {
        targetDir = targetPos - npcMovement.transform.position;
    }

    protected override void DoStartActivity()
    {
        isActive = true;
        timePassed = 0;
        previousYAngle = GetAngleFromDirection(npcMovement.GetLookDirection());
        nextYAngle = GetRandomAngleWithin(targetDir, checkRadius);
        timer.Init(changeAngleTime, UpdateNextTargetDir);
    }

    protected override void DoStopActivity()
    {
        isActive = false;
        timer.Stop();
    }

    protected override bool IsFinished()
    {
        return timePassed >= checkDuration;
    }

    private void UpdateNextTargetDir()
    {
        previousYAngle = nextYAngle;
        nextYAngle = GetRandomAngleWithin(targetDir, checkRadius);

        timePassed += timer.GetTimePassed();
        timer.Init(changeAngleTime, UpdateNextTargetDir);
    }

    private static float GetRandomAngleWithin(Vector3 targetCenter, float checkRadius)
    {
        float targetYAngle = GetAngleFromDirection(targetCenter); 
        float randomYAngle = Random.Range(targetYAngle - checkRadius, targetYAngle + checkRadius);
        return randomYAngle;
    }

    private static float GetAngleFromDirection(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
