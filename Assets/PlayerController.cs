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

    public void SetupJump(float jumpHeight, float jumpTime) {
        jumpVelocity = 2 * jumpHeight / jumpTime;
        gravityScale = (jumpVelocity / jumpTime) / -Physics2D.gravity.y;
    }
    
    public MovementMode(string name, float jumpHeight, float JumpTime) {
        this.name = name;
        SetupJump(jumpHeight, JumpTime);
    }
    public MovementMode(string name) {
        this.name = name;
    }
}//put jymo height and jukp speed ijto the class.

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    
    public InputAction moveAction;
    public InputAction jumpAction;

    public LayerMask groundLayers;

    public Vector2 movement;

    //public bool doHover;
    //private float jumpVelocity;
    // public float hoverGravityScale;
    // public float hoverDrag;
    public enum Mode : int {
        Water,
        Underground,
        Ice,
        Cloud,
    }

    public MovementMode[] modes = {
        new MovementMode("Water", 3.1f, 0.9f),
        new MovementMode("Underground"){ gravityScale = 0.0f },
        new MovementMode("Ice", 1.05f, 0.5f),
        new MovementMode("Cloud"){ gravityScale = 0.1f },
    };
    public Mode currentMode = Mode.Water;
    public MovementMode CurrentMode { get => modes[(int)currentMode]; }

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
            mode.SetupJump();///////////err
        }
    }
    
    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {///////err
            rb.gravityScale = hoverGravityScale;//////err
            rb.drag = hoverDrag;///////err

            if (movement.x != 0)
                rb.velocityX = movement.x * speed;///////err

            if (movement.y != 0)
                rb.velocityY = movement.y * speed;///////err

        }
        else {
            rb.gravityScale = normalGravityScale;///////err
            rb.drag = 0;

            rb.velocityX = movement.x * speed;///////err

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = jumpVelocity;///////err
            }

        }

    }

    private bool IsOnGround()
    {
        return rb.IsTouchingLayers(groundLayers);
    }

}
