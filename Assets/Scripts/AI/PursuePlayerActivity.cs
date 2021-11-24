using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursuePlayerActivity : AbstractActivity
{
    private const float TARGET_REACHED_RANGE = 1.0f;

    [SerializeField]
    private float playerReachedRange = 2.0f;

    [SerializeField]
    private float moveSpeed = 2;

    [SerializeField]
    private float playerPosUpdateRate = 0.75f;

    private NavMeshAgent navMeshAgent;

    private Vector3? updatedPos = null;
    private Vector3 nextPosition;

    private Timer timer;

    private bool lostPlayer = false;
    private Transform playerTransform = null;
    void Start()
    {
        this.order = -1; //not needed
        this.baseAlert = 1; //not needed

        timer = GetComponent<Timer>();
        navMeshAgent = controlledGameObject.GetComponent<NavMeshAgent>();
    }

    protected override void DoStartActivity()
    {
        navMeshAgent.isStopped = false; 
        UpdateDestinationFromTargetPosition();
        navMeshAgent.speed = moveSpeed;
        lostPlayer = false;
    }

    protected override void DoStopActivity()
    {
        navMeshAgent.isStopped = true;
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
        navMeshAgent.SetDestination(GetClosestPositionFor(updatedPos.Value));
        updatedPos = null;
        timer.Init(playerPosUpdateRate, UpdateDestinationFromTargetPosition);
    }

    public override bool IsFinished()
    {
        if (PlayerIsReached())
        {
            timer.Stop();
            GameManager.Instance.SetGameOver(GameManager.GameOverReason.CoverBlown);
        }

        if (TargetIsReached() && !lostPlayer)
        {
            if (updatedPos.HasValue)
            {
                navMeshAgent.SetDestination(GetClosestPositionFor(updatedPos.Value));
            }
        }

        /*if (lostPlayer)
        {

        }*/
        
        return lostPlayer;
    }

    private Vector3 GetPos()
    {
        return controlledGameObject.transform.position;
    }

    private bool TargetIsReached()
    {
        return Vector3.Distance(GetPos(), nextPosition) <= TARGET_REACHED_RANGE;
    }

    private bool PlayerIsReached()
    {
        return Vector3.Distance(GetPos(), playerTransform.position) <= playerReachedRange;
    }

    public override void SetPaused(bool isPaused)
    {
        this.navMeshAgent.speed = isPaused ? 0 : moveSpeed;
    }
}