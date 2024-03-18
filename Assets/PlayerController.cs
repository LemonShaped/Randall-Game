using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementMode
{
    public string name;

    public float speed;
    
    public float jumpVelocity;
    public float gravityScale;

    public SetupJump(float jumpHeight, float jumpTime) {
        this.jumpVelocity = 2 * jumpHeight / jumpTime;
        this.gravityScale = (this.jumpVelocity / jumpTime) / -Physics2D.gravity.y;
    }
    
    public MovementMode(string name, float jumpHeight, float JumpTime) {
        this.name = name;
        this.SetupJump(jumpHeight, JumpTime)
    }
    public MovementMode(string name) {
        this.name = name;
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

    private float jumpVelocity;
    
    // private float normalGravityScale;

    // public float hoverGravityScale;
    // public float hoverDrag;
    public enum Mode {
        Water,
        Underground,
        Ice,
        Cloud,
    }
    public MovementMode[4] modes = {
        new MovementMode("Water", 3.1f, 0.9f),
        new MovementMode("Underground"){ gravityScale = 0.0f }
        new MovementMode("Ice", 1.05f, 0.5f),
        new MovementMode("Cloud"){ gravityScale = 0.1f },
    };
    public Mode currentMode = Mode.Water;
    public MovementMode CurrentMode { get => return modes[currentMode]; };
    
    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void OnValidate()
    {
        foreach (MovementMode mode in modes) {
            mode.SetupJump();
        }
    }
    
    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {
            rb.gravityScale = hoverGravityScale;
            rb.drag = hoverDrag;

            if (movement.x != 0)
                rb.velocityX = movement.x * speed;

            if (movement.y != 0)
                rb.velocityY = movement.y * speed;

        }
        else {
            rb.gravityScale = normalGravityScale;
            rb.drag = 0;

            rb.velocityX = movement.x * speed;

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
