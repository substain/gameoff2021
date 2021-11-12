using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Tooltip("How fast the player is by walking normally")]
    [SerializeField]
    private float baseMovementSpeed = 4f;

    [Tooltip("The factor how fast the player is when sneaking compared to walking")]
    [SerializeField]
    float sneakMovementFactor = 0.5f;

    [Tooltip("The factor how fast the player is when running compared to walking")]
    [SerializeField]
    float runMovementFactor = 1.75f;

    [Tooltip("The lowest input movement amount before it will be ignored")]
    [SerializeField]
    float movementInputCutoff = 0.025f;

    [Tooltip("The fraction of the movement input that adds to the velocity")]
    [SerializeField]
    float velocityBuildupFraction = 0.3f;

    [Tooltip("The fraction of the velocity that is decreased from time to time")]
    [SerializeField]
    float velocityDecreaseFraction = 0.1f;

    [Tooltip("The maximum velocity that can be build up")]
    [SerializeField]
    float maxVelocity = 2f;

    [Tooltip("The lowest velocity amount before it will be set to 0")]
    [SerializeField]
    float velocityCutoff = 0.005f;

    [Tooltip("The duration of the dash action")]
    [SerializeField]
    float dashDuration = 0.17f;

    [Tooltip("The dash strength/force/speed")]
    [SerializeField]
    float dashForce = 35f;

    [Tooltip("The relative point from which dash speed is decreased (between 0 and 1). Leads to a short pause after the dash if very small. ")]
    [SerializeField]
    float dashCutoffPoint = 0.9f;

    [Tooltip("The amount of decrease per frame after the dashCutoffPoint is reached.")]
    [SerializeField]
    float dashDecreaseFraction = 0.85f;

    [Tooltip("The time it takes to reload the dash after it was used")]
    [SerializeField]
    float dashReloadDuration = 0.8f;


    private Vector3 velocity = Vector3.zero;
    private Vector3 currentMovement = Vector3.zero;
    
    private Rigidbody rigidBody;
    private SpriteRenderer spriteRenderer;

    private bool isRunning = false;
    private bool isSneaking = false;

    private float movementModifier = 1;

    private float currentDashStrength = 0;

    private bool isDashing;
    private Timer dashTimer;

    private bool isPaused = false;

    private Vector3 lastMoveDir = new Vector3(1, 0, 0);

    void Awake()
    {
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        this.rigidBody = GetComponent<Rigidbody>();
        this.dashTimer = gameObject.AddComponent<Timer>();
    }

    void FixedUpdate()
    {
        ApplyVelocity();

        if (isDashing)
        {
            ProcessDashMovement();
        }
        else
        {
            ProcessMovement();
        }
    }

    private void ApplyVelocity()
    {
        velocity *= 1 - velocityDecreaseFraction;

        if (velocity.magnitude < velocityCutoff)
        {
            velocity = Vector3.zero;
            return;
        }
    }

    public void ProcessRunInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = true;
        }
        if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void ProcessSneakInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isSneaking = true;
        }
        if (context.canceled)
        {
            isSneaking = false;
        }
    }



    public void ProcessMoveInput(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            return;
        }
        ProcessMoveInput(context.ReadValue<Vector2>());
    }

    private void ProcessMoveInput(Vector2 direction)
    {
        Vector3 moveDirection = Util.ToVector3(direction);
        if (moveDirection.magnitude < movementInputCutoff)
        {
            currentMovement = Vector3.zero;
            return;
        }

        if (isDashing)
        {
            //cant move while dashing
            return;
        }

        moveDirection = moveDirection.normalized;
        lastMoveDir = moveDirection;

        movementModifier = GetMovementModifier();

        float moveSpeed = baseMovementSpeed * movementModifier;
        currentMovement = moveDirection * moveSpeed;
        UpdateSpriteByMoveVector(currentMovement);
    }

    private float GetMovementModifier()
    {
        float movementModifier = 1;

        if (isSneaking)
        {
            movementModifier = sneakMovementFactor;
        }

        //running overrides sneaking
        if (isRunning)
        {
            movementModifier = runMovementFactor;
        }
        return movementModifier;
    }

    private void UpdateSpriteByMoveVector(Vector3 moveVector)
    {
        //spriteRenderer.flipX = moveVector.x > 0;
        //if(moveVector.)
    }

    private void ProcessMovement()
    {        
        //add a fraction of the movement to the velocity
        velocity += velocityBuildupFraction * currentMovement;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity * movementModifier);

        //execute movement
        rigidBody.velocity = currentMovement + velocity;
    }

    private void ProcessDashMovement()
    {
        bool fullDash = dashTimer.GetRelativeProgress() < dashCutoffPoint;

        currentDashStrength = fullDash ? currentDashStrength : currentDashStrength * dashDecreaseFraction;
        //execute movement
        Vector3 dashMovement = lastMoveDir.normalized * currentDashStrength * dashForce;
        if (!fullDash)
        {
            velocity += velocityBuildupFraction * dashMovement;
            velocity = Vector3.ClampMagnitude(velocity, maxVelocity * movementModifier);
            dashMovement += velocity;
        }

        rigidBody.velocity = dashMovement;
    }


    public void ProcessDashInput(InputAction.CallbackContext context)
    {
        if (isPaused || isDashing || dashTimer.IsRunning())
        {
            return;
        }
        isDashing = true;
        dashTimer.Init(dashDuration, SetDashFinished);
        currentDashStrength = GetMovementModifier();
    }

    public void SetDashFinished()
    {
        isDashing = false;
        dashTimer.Init(dashReloadDuration);
        currentDashStrength = 0;
    }

    public void Pause()
    {
        isPaused = true;
        dashTimer.SetPaused(true);
    }

    public void Unpause()
    {
        isPaused = false;
        dashTimer.SetPaused(false);
    }
}

