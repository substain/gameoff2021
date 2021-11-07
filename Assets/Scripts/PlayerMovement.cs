using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private KeyCode runKey = KeyCode.LeftShift;

    [SerializeField]
    private KeyCode sneakKey = KeyCode.LeftControl;

    [Tooltip("How fast the player is by walking normally")]
    [SerializeField]
    private float baseMovementSpeed = 80f;

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
    float maxVelocity = 50f;

    [Tooltip("The lowest velocity amount before it will be set to 0")]
    [SerializeField]
    float velocityCutoff = 0.015f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentMovement = Vector3.zero;
    
    private Rigidbody rigidBody;

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyVelocity();
        ProcessMoveInput();
        ProcessMovement();
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

    private void ProcessMoveInput()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if(moveDirection.magnitude < movementInputCutoff)
        {
            currentMovement = Vector3.zero;
            return;
        }

        float movementModifier = 1;

        if (Input.GetKey(sneakKey)){
            movementModifier = sneakMovementFactor;
        }

        //running overrides sneaking
        if (Input.GetKey(runKey))
        {
            movementModifier = runMovementFactor;
        }

        float moveSpeed = baseMovementSpeed * movementModifier;
        currentMovement = moveDirection * Time.fixedDeltaTime * moveSpeed;

        //add a fraction of the movement to the velocity
        velocity += velocityBuildupFraction * currentMovement;

        Vector3.ClampMagnitude(velocity, maxVelocity * movementModifier);
    }

    private void ProcessMovement()
    {
        rigidBody.velocity = currentMovement + velocity;
    }
}

