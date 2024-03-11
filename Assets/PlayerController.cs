using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementMode
{
    public string name;
    public float jumpVelocity;
    public float gravityScale;
    
    public float jumpHeight;
    public float jumpTime;
    
    public void setupJump() {
        jumpVelocity = 2 * jumpHeight / jumpTime;
        gravityScale = (jumpVelocity / jumpTime) / -Physics2D.gravity.y;
    }
}

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    
    public InputAction moveAction;
    public InputAction jumpAction;

    public LayerMask groundLayers;

    public Vector2 movement;

    public bool doHover;
    public float walkSpeed; // walking speed in m/s

    public float jumpHeight;
    public float jumpTime;

    private float jumpVelocity;
    
    private float normalGravityScale;

    public float hoverGravityScale;
    public float hoverDrag;

    public class Modes {
        MovementMode Ice = new MovementMode() { jumpHeight = 1.05f, jumpTime = 0.5f };
        MovementMode Water = new MovementMode() { jumpHeight = 3.1f, jumpTime = 0.9f};
        MovementMode Cloud = new MovementMode();
    };
    public MovementMode currentMode;
    
    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        foreach (MovementMode mode in Modes) { // error
            mode.setupJump();
        } 
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void OnValidate()
    {
        foreach (MovementMode mode in Modes) { //
            mode.setupJump();
        }
    }
    
    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {
            rb.gravityScale = hoverGravityScale;
            rb.drag = hoverDrag;

            if (movement.x != 0)
                rb.velocityX = movement.x * walkSpeed;

            if (movement.y != 0)
                rb.velocityY = movement.y * walkSpeed;

        }
        else {
            rb.gravityScale = normalGravityScale;
            rb.drag = 0;

            rb.velocityX = movement.x * walkSpeed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = jumpVelocity;
            }

        }

    }

    private bool IsOnGround()
    {
        return rb.IsTouchingLayers(groundLayers);
    }

}
