using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementActivity : AbstractActivity
{
    private const float TARGET_REACHED_RANGE = 1.0f; 
    private const float DOOR_REACHED_RANGE = 2.0f;

    private const float KEEP_DOOR_OPEN_TIME = 0.5f;
    private const float PASS_DOOR_SPEED = 0.75f;

    [SerializeField]
    private Transform targetPosition;

    [SerializeField]
    private float moveSpeed = 2;

    [SerializeField]
    List<Door> doorsToPass = new List<Door>();

    [SerializeField]
    private bool closeDoorAfterEnter = false;

    private NavMeshAgent navMeshAgent;

    private Vector3 closestPosition;

    private Timer timer;

    void Start()
    {
        timer = GetComponent<Timer>();
        navMeshAgent = controlledGameObject.GetComponent<NavMeshAgent>();
        SetClosestPosition();
        InvokeRepeating("CheckForDoors", 1, 0.1f);
    }

    void Update()
    {
        CheckForDoors();
    }

    private void CheckForDoors()
    {
        if (!activityActive)
        {
            return;
        }

        foreach (Door door in doorsToPass)
        {
            //check if there are any near doors from the ones in the list
            if (Vector3.Distance(GetPos(), door.gameObject.transform.position) < DOOR_REACHED_RANGE)
            {
                //open it 
                door.OpenDoor();
                timer.Init(KEEP_DOOR_OPEN_TIME, delegate { PassDoor(door); });
            }
        }
    }

    private void PassDoor(Door currentDoor)
    {
        if (closeDoorAfterEnter)
        {
            currentDoor.CloseDoor();
        }
    }

    /// <summary>
    /// make sure the pos to walk to is on the navmesh
    /// </summary>
    private void SetClosestPosition()
    {
        NavMeshHit closestNavMeshPosition;
        NavMesh.SamplePosition(targetPosition.position, out closestNavMeshPosition, 2.0f, NavMesh.AllAreas);
        closestPosition = closestNavMeshPosition.position;
    }

    protected override void DoStartActivity()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(closestPosition);
        navMeshAgent.speed = moveSpeed;
    }
    protected override void DoStopActivity()
    {
        navMeshAgent.isStopped = true;
    }

    public override bool IsFinished()
    {
        return TargetIsReached();
    }

    private bool TargetIsReached()
    {
        return Vector3.Distance(GetPos(), closestPosition) <= TARGET_REACHED_RANGE;
    }

    public override void SetPaused(bool isPaused)
    {
        this.navMeshAgent.speed = isPaused ? 0 : moveSpeed;
        //navMeshAgent.isStopped = false;
    }

    private Vector3 GetPos()
    {
        return controlledGameObject.transform.position;
    }
}