using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : LiquidCharacter
{

    public float hurtTimeout;
    public float hurtTimeoutRemaining;

    public InputAction moveAction;
    public InputAction jumpAction;

    private new void Awake()
    {
        base.Awake();
        canPickUpPuddles = true;
        canInteractWithStateChangerObjects = true;
        isHurtByEthanol = true;
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    public override bool Hurt()
    {
        if (hurtTimeoutRemaining <= 0) {
            hurtTimeoutRemaining = hurtTimeout;
            return AddHealth(-1);
        }
        return false;
    }

    private void FixedUpdate()
    {
        hurtTimeoutRemaining -= Time.fixedDeltaTime;

        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        rb.drag = ModeData.drag * SizeData.dragMultiplier;

        if (movementInput.x < 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;

        // jump
        if (ModeData.doesJump && jumpAction.IsPressed() && IsOnGround()) {
            rb.velocityY = jumpVelocity;
        }

        if (CurrentMode == ModesEnum.Water) {

            rb.velocityX = movementInput.x * movementSpeed;

        }
        else if (CurrentMode == ModesEnum.Ice) {

            if (movementInput.x != 0)
                rb.velocityX = movementInput.x * movementSpeed;

        }
        else if (CurrentMode == ModesEnum.Cloud || CurrentMode == ModesEnum.Water_Underground) {

            if (movementInput.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movementInput.x * movementSpeed;

            if (movementInput.y != 0)
                rb.velocityY = movementInput.y * movementSpeed;

        }

        if ((CurrentMode == ModesEnum.Water || CurrentMode == ModesEnum.Water_Underground)
                && movementInput.y < 0 && groundCheck.CheckGround(groundLayers) && IsPorous(GridPosition + Vector3Int.down)) {
            CurrentMode = ModesEnum.Water_Underground;
        }
        else if ((CurrentMode == ModesEnum.Water_Underground) && groundTilemap.GetTile(GridPosition) == null) {
            CurrentMode = ModesEnum.Water;
        }

    }
}
