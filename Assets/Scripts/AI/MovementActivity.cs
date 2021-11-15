using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementActivity : AbstractActivity
{
    private const float TARGET_REACHED_RANGE = 1.5f;
    private const float KEEP_DOOR_OPEN_TIME = 0.75f; 
    private int DOOR_RECHECK_AMOUNT = 2;

    [SerializeField]
    private Transform targetPosition;

    [SerializeField]
    private float moveSpeed = 2;

    [SerializeField]
    List<Door> doorsToPass = new List<Door>();

    private bool closeDoorAfterEnter = false;

    private NavMeshAgent navMeshAgent;

    private Vector3 closestPosition;

    private Vector3 lastPos;

    private Timer timer;

    private int doorRecheckCounter;

    void Start()
    {
        timer = GetComponent<Timer>();
        lastPos = GetPos();
        navMeshAgent = controlledGameObject.GetComponent<NavMeshAgent>();
        SetClosestPosition();
        InvokeRepeating("CheckForDoors", 1, 0.25f);
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

        Vector3 currentPos = GetPos();
        //No movement since last check -> maybe a door is in the way
        if (lastPos.Equals(currentPos))
        {
            doorRecheckCounter++;
           
            if(doorRecheckCounter >= DOOR_RECHECK_AMOUNT)
            {
                doorRecheckCounter = 0;
                Debug.Log("lastPos:" + lastPos + ", currentPos = " + currentPos);
                foreach (Door door in doorsToPass)
                {
                    //check if there are any near doors from the ones in the list
                    if (Vector3.Distance(GetPos(), door.gameObject.transform.position) < 1f)
                    {
                        //open it 
                        door.OpenDoor();

                        //auto-close it after a fixed time
                        if (closeDoorAfterEnter)
                        {
                            timer.Init(KEEP_DOOR_OPEN_TIME, () => door.CloseDoor());
                        }
                    }
                }
            }

        }
        else
        {
            doorRecheckCounter = 0;
            lastPos = currentPos;
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
        return Vector3.Distance(GetPos(), closestPosition) < TARGET_REACHED_RANGE;
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