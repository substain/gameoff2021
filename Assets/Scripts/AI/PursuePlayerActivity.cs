using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursuePlayerActivity : AbstractActivity
{
    private const float TARGET_REACHED_RANGE = 1.0f;

    [SerializeField]
    private float playerReachedRange = 2.0f;

    [SerializeField]
    private float moveSpeed = 7;

    [SerializeField]
    private float playerPosUpdateRate = 0.75f;

    private NPCMovement npcMovement;

    private Vector3? updatedPos = null;

    private Timer timer;

    private bool lostPlayer = false;
    private Transform playerTransform = null;

    [SerializeField]
    private float listenTimeNeededForConstraint = float.MaxValue;

    void Start()
    {
        this.order = -1; //not needed
        playerPosUpdateRate = playerPosUpdateRate * 1 / SettingsManager.GetDifficultyModifier();
        moveSpeed = moveSpeed * SettingsManager.GetDifficultyModifier();

        timer = GetComponent<Timer>();
        npcMovement = controlledGameObject.GetComponent<NPCMovement>();
    }

    protected override void DoStartActivity()
    {
        UpdateDestinationFromTargetPosition();
        npcMovement.StartMovement(moveSpeed);
        lostPlayer = false;
    }

    protected override void DoStopActivity()
    {
        npcMovement.StopMovement();
    }

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        updatedPos = newPosition;
    }
    
    public void UpdateDestinationFromTargetPosition()
    {
        if (!updatedPos.HasValue)
        {
            lostPlayer = true;
            timer.Stop();
            return;
        }
        npcMovement.SetMoveTarget(updatedPos.Value);
        updatedPos = null;
        timer.Init(playerPosUpdateRate, UpdateDestinationFromTargetPosition);
    }

    protected override bool IsFinished()
    {
        if (PlayerIsReached())
        {
            timer.Stop();
            GameManager.GameInstance.SetGameOver(GameManager.GameOverReason.CoverBlown);
            //GetComponent<ActivityManager>().SetPaused(true);
            return true;
        }

        if (TargetIsReached() && !lostPlayer)
        {
            if (updatedPos.HasValue)
            {
                npcMovement.SetMoveTarget(updatedPos.Value);
            }
        }

        /*if (lostPlayer)
        {
            //Wait and look around for a short while?
        }*/
        
        return lostPlayer;
    }

    private bool TargetIsReached()
    {
        return npcMovement.IsWithinDistanceToDestination(TARGET_REACHED_RANGE);
    }

    private bool PlayerIsReached()
    {
        return npcMovement.IsWithinDistance(playerTransform.position, playerReachedRange);
    }

    public override void SetPaused(bool isPaused)
    {
        npcMovement.SetPaused(true);
        base.SetPaused(isPaused);
    }

    public override bool IsContinuous()
    {
        return true;
    }

    public override float GetNeededTimeToListen()
    {
        return listenTimeNeededForConstraint;
    }
}