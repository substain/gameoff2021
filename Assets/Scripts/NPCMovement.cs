using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private WatcherNPC watcherNpc;

    [SerializeField]
    private Vector3 lookDirection = Vector3.back;

    private float currentSpeed = 0;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        watcherNpc = GetComponent<WatcherNPC>();
    }

    void Start()
    {
        SetLookDirection(lookDirection);
    }

    void FixedUpdate()
    {
        UpdateSpriteByMovement();
    }

    public void StartMovement(float? speed = null)
    {
        animator.SetBool("isWalking", true);

        navMeshAgent.isStopped = false;
        if (speed.HasValue)
        {
            currentSpeed = speed.Value;
            navMeshAgent.speed = currentSpeed;
        }
        //animator.SetBool("isWalking", true);
    }

    public void SetMoveTarget(Vector3 target)
    {
        navMeshAgent.SetDestination(GetClosestPositionFor(target));
    }

    public void StopMovement()
    {
        navMeshAgent.isStopped = true; 
        animator.SetBool("isWalking", false);
    }

    /// <summary>
    /// make sure the pos to walk to is on the navmesh
    /// </summary>
    public static Vector3 GetClosestPositionFor(Vector3 targetPos)
    {
        NavMeshHit closestNavMeshPosition;
        NavMesh.SamplePosition(targetPos, out closestNavMeshPosition, 2.0f, NavMesh.AllAreas);
        return closestNavMeshPosition.position;
    }

    public void SetPaused(bool isPaused)
    {
        this.navMeshAgent.speed = isPaused ? 0 : currentSpeed;
    }

    public bool IsWithinDistance(Vector3 target, float dist)
    {
        return Vector3.Distance(transform.position, target) <= dist;
    }

    private void UpdateSpriteByMovement()
    {
        if (navMeshAgent.isStopped)
        {
            return;
        }
        //make sure y movement is ignored here
        Vector2 velo2D = Util.ToVector2XZ(navMeshAgent.velocity);
        animator.speed = velo2D.magnitude / navMeshAgent.speed;

        if (velo2D.magnitude > 0.1f)
        {
            SetLookDirection(Util.ToVector3(velo2D));
        }
    }

    /// <summary>
    /// Sets the view direction. Will be used in fixedupdate while moving, so setting this from outside the npc movement class 
    /// is only recommended for when the npc is not moving.
    /// </summary>
    public void SetLookDirection(Vector3 lookDir)
    {
        spriteRenderer.flipX = lookDir.x < 0;
        animator.SetBool("isBack", lookDir.z > 0);
        watcherNpc.SetLookDirection(lookDir);
        lookDirection = lookDir;
    }

    public Vector3 GetLookDirection()
    {
        return lookDirection;
    }
}
