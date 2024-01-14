using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public InputAction moveAction;
    public InputAction jumpAction;

    public LayerMask groundLayers;

    public Vector2 movement;

    public bool doHover;
    public float walkSpeed; // walking speed in m/s
    public float jumpVelocity;

    public float jumpHeight;
    public float jumpTime;

    public float normalGravityScale;
    public float hoverGravityScale;

    private void OnEnable()
    {
        moveAction.Enable();

        OnValidate();
    }

    private void OnValidate()
    {
        jumpVelocity = 2 * jumpHeight / jumpTime;
        normalGravityScale = (jumpVelocity / jumpTime) / -Physics2D.gravity.y;
    }
    // s=s  u=?  v=0  a=  t=t
    // s = (v+u)/2 * t
    // u = 2s/t - 0
    // jumpVelocity = 2*jumpHeight/jumpTime

    // v = u + at
    // a=(v-u)/t
    // a= -u/t
    // gravity = -jumpVelocity/jumpTime

    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {
            rb.gravityScale = hoverGravityScale;
            rb.drag = 5f;

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
            if (rb.IsTouchingLayers(groundLayers) && jumpAction.IsPressed()) {
                rb.velocityY = jumpVelocity;
            }

        }

    }

}
