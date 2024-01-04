using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public InputAction moveAction; // this has an X and a Y component, but we only use the Y axis if we are flying.
    public InputAction jumpAction;
    public Vector2 move;
    public Rigidbody2D rb;

    public float walkSpeed; // walk speed in m/s
    public float jumpVelocity;
    public bool doHover;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        jumpAction.performed += OnJump; // we are using a callback so that we dont have to constantly check if whether is pressed.
    }

    private void Update()
    {
        move = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (doHover)
            rb.velocity = move * walkSpeed;
        else
            rb.velocityX = Mathf.Ceil(move.x) * walkSpeed; // we ceil it to 1 because move.x is 1.4 if we are holding both sideways and jump
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!doHover)
            rb.velocityY = jumpVelocity;
    }
}
