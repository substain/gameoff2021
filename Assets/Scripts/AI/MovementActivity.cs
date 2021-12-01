using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementActivity : AbstractActivity
{
    private const float TARGET_REACHED_RANGE = 1.5f; 
    private const float DOOR_REACHED_RANGE = 2.0f;

    private const float KEEP_DOOR_OPEN_TIME = 0.3f;
    private const float PASS_DOOR_SPEED = 0.75f;

    [SerializeField]
    private Transform targetPosition;

    [SerializeField]
    private float moveSpeed = 2;

    [SerializeField]
    List<Door> doorsToPass = new List<Door>();

    [SerializeField]
    private bool closeDoorAfterEnter = false;

    private NPCMovement npcMovement;

    private Vector3 closestPosition;

    private Timer timer;

    [SerializeField]
    private float listenTimeNeededForConstraint = float.MaxValue;

    void Awake()
    {
        npcMovement = controlledGameObject?.GetComponent<NPCMovement>();
    }

    void Start()
    {
        npcMovement = controlledGameObject?.GetComponent<NPCMovement>();
        timer = GetComponent<Timer>();
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

    protected override void DoStartActivity()
    {
        npcMovement = controlledGameObject?.GetComponent<NPCMovement>();
        npcMovement.SetMoveTarget(targetPosition.position);
        npcMovement.StartMovement(moveSpeed);
    }

    protected override void DoStopActivity()
    {
        npcMovement.StopMovement();
    }

    protected override bool IsFinished()
    {
        return npcMovement.IsWithinDistanceToDestination(TARGET_REACHED_RANGE);
    }

    public override void SetPaused(bool isPaused)
    {
        npcMovement.SetPaused(isPaused);
        base.SetPaused(isPaused);
    }

    private Vector3 GetPos()
    {
        return controlledGameObject.transform.position;
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