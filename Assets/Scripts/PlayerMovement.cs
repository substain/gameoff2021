using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    private const float DASH_SLIDER_FADOUT_TIME = 0.35f;

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

    [SerializeField]
    private List<AudioClip> footstepClips;

    [SerializeField]
    private List<AudioClip> dashClips;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentMovement = Vector3.zero;
    
    private Rigidbody rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isRunning = false;
    private bool isSneaking = false;

    private float movementModifier = 1;

    private float currentDashStrength = 0;

    private bool isDashing;
    private Timer dashTimer;
    private Timer footstepTimer;
    private Timer hideSliderTimer;
    private Vector3 lastMoveDir = new Vector3(1, 0, 0);
    private CanvasGroup dashSliderGroup;

    private int numCheeseEaten = 0;
    private Timer cheeseTimer;

    private Slider dashSlider;

    private bool isInBlockingDialogue = false;
    private bool isInMenu = false;
    private bool playFootsteps = false;

    private AudioSource playerSource;

    void Awake()
    {
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        this.rigidBody = GetComponent<Rigidbody>();
        this.dashTimer = gameObject.AddComponent<Timer>();
        this.footstepTimer = gameObject.AddComponent<Timer>();
        this.hideSliderTimer = gameObject.AddComponent<Timer>();
        this.cheeseTimer = gameObject.AddComponent<Timer>();
        this.animator = GetComponentInChildren<Animator>();
        this.dashSlider = GetComponentInChildren<Slider>();
        this.dashSliderGroup = GetComponentInChildren<CanvasGroup>();
        playerSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        HideDashSlider();
        Vector3? startPos = GameManager.GameInstance.GetPlayerStartPos();
        if (startPos.HasValue)
        {
            transform.position = startPos.Value;
        }
    }

    void FixedUpdate()
    {
        ApplyVelocity();
        UpdateDashSlider();

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

    public void AddCheeseEaten()
    {
        numCheeseEaten++;
        float cheeseDuration = 0.75f + numCheeseEaten * 0.75f;
        cheeseTimer.Init(cheeseDuration);
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

    public bool IsQuiet()
    {
        return !isDashing && (isSneaking || currentMovement.magnitude < 2 * movementInputCutoff);
    }

    public void ProcessMoveInput(InputAction.CallbackContext context)
    {
        if (isInMenu)
        {
            currentMovement = Vector3.zero;
            animator.SetBool("isWalking", false);
            playFootsteps = false;
            return;
        }
        ProcessMoveInput(context.ReadValue<Vector2>());
    }

    private void ProcessMoveInput(Vector2 direction)
    {
        Vector3 moveDirection = Util.ToVector3(direction);

        if (isInBlockingDialogue || moveDirection.magnitude < movementInputCutoff)
        {
            animator.SetBool("isWalking", false);
            playFootsteps = false;
            currentMovement = Vector3.zero;
            return;
        }

        if (isDashing)
        {
            //cant move while dashing
            return;
        }
        animator.SetBool("isWalking", true);
        StartPlayingFootsteps();

        moveDirection = moveDirection.normalized;
        lastMoveDir = moveDirection;
        currentMovement = moveDirection;
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
        spriteRenderer.flipX = moveVector.x < 0;
        animator.SetBool("isBack", moveVector.z > 0);
    }

    private void ProcessMovement()
    {
        float slowdownFactor = cheeseTimer.IsRunning() ? cheeseTimer.GetRelativeProgress() : 1;
        movementModifier = GetMovementModifier() * slowdownFactor;
        animator.speed = movementModifier;
        float moveSpeed = baseMovementSpeed * movementModifier;

        Vector3 usedMovement = currentMovement * moveSpeed;

        //add a fraction of the movement to the velocity
        velocity += velocityBuildupFraction * usedMovement;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity * movementModifier);

        //execute movement
        rigidBody.velocity = Util.Vector3MinY(0.15f, Util.CombineVectorsXZandY(usedMovement + velocity, rigidBody.velocity));

    }

    public void ProcessDashInput(InputAction.CallbackContext context)
    {
        if (isInMenu || !context.performed)
        {
            return;
        }
        if (isInBlockingDialogue || isDashing || dashTimer.IsRunning() || cheeseTimer.IsRunning())
        {
            return;
        }
        isDashing = true;
        dashTimer.Init(dashDuration, SetDashFinished);
        currentDashStrength = 1; //GetMovementModifier() was making dashes too far
        animator.speed = 1.0f;
        animator.SetBool("isWalking", false); 
        playFootsteps = false;
        animator.SetTrigger("dash");

        if (dashClips.Count > 0)
        {
            playerSource.PlayOneShot(Util.ChooseRandomFromList(dashClips));
        }
        else
        {
            playerSource.Stop();
        }
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

    public void SetDashFinished()
    {
        bool isWalking = currentMovement.magnitude >= movementInputCutoff;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            StartPlayingFootsteps();
        }

        isDashing = false;
        hideSliderTimer.Stop();
        dashTimer.Init(dashReloadDuration, HideDashSliderSlow);
        currentDashStrength = 0;
    }

    public void SetMenuActive(bool isInMenu)
    {
        this.isInMenu = isInMenu;
        currentMovement = Vector3.zero;
    }

    public void SetBlockingDialogueActive(bool isBlocked)
    {
        this.isInBlockingDialogue = isBlocked;
        currentMovement = Vector3.zero;
    }

    private void UpdateDashSlider()
    {
        if (!isDashing && dashTimer.IsRunning())
        {
            dashSlider.value = dashTimer.GetRelativeProgress();
            dashSliderGroup.alpha = dashTimer.GetRelativeProgress();
        }

        if (hideSliderTimer.IsRunning())
        {
            dashSliderGroup.alpha = 1-hideSliderTimer.GetRelativeProgress();
        }
    }

    private void HideDashSliderSlow()
    {
        hideSliderTimer.Init(DASH_SLIDER_FADOUT_TIME, HideDashSlider);
    }

    private void HideDashSlider()
    {
        dashSlider.value = 0;
        dashSliderGroup.alpha = 0;
    }

    private void StartPlayingFootsteps()
    {
        if (playFootsteps)
        {
            return;
        }
        playFootsteps = true;
        PlayFootstepsRepeated();
    }

    private void PlayFootstepsRepeated()
    {
        if (!playFootsteps)
        {
            return;
        }
        playerSource.PlayOneShot(Util.ChooseRandomFromList(footstepClips));
        float delay = 0.2f;
        if (isSneaking)
        {
            delay = 0.4f;
        }
        footstepTimer.Init(delay, PlayFootstepsRepeated);
    }

    public bool SlowedByCheese()
    {
        return cheeseTimer.IsRunning();
    }
}

