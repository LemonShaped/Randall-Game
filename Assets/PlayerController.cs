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

    private void OnEnable()
    {
        moveAction.Enable();

        if (jumpHeight > 0) {
            jumpVelocity = 2 * jumpHeight / jumpTime; //Mathf.Sqrt(2 * -Physics2D.gravity.y * jumpHeight); //without jumpTime
            rb.gravityScale = (-jumpVelocity / jumpTime) / Physics2D.gravity.y;
        }
    }
    /* s=s u=? v=0 a=-g t=
       v^2 = u^2 + 2as
       u = sqrt(-2as)
       jumpVelocity = sqrt(2*9.8*jumpHeight)
    */


    // s=s  u=?  v=0  a=?  t=t
    // s = (v+u)/2 * t
    // u = 2s/t - 0
    // jumpVelocity = 2*jumpHeight/jumpTime
    //
    // v = u + at
    // a=(v-u)/t
    // a= -u/t
    // gravity = -jumpVelocity/jumpTime
    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {
            rb.gravityScale = 0.2f;
            rb.drag = 5f;

            if (movement.x != 0)
                rb.velocityX = movement.x * walkSpeed;

            if (movement.y != 0)
                rb.velocityY = movement.y * walkSpeed;

        }
        else {
            rb.gravityScale = 1;
            rb.drag = 0;

            rb.velocityX = movement.x * walkSpeed;

            // jump
            if (movement.y > 0 && rb.IsTouchingLayers(groundLayers)) {
                rb.velocityY = jumpVelocity;
            }

        }

    }

}
