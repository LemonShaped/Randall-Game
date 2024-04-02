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

    private Vector2 movementInput;

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

    public override void Die() {
        gameManager.LevelFailed();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
            Hurt();
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Puddle")) {
            bool changed = AddHealth(1);
            if (changed)
                Destroy(collider.gameObject);
        }
        else if (collider.gameObject.TryGetComponent(out PhaseChangingObject phaseChanger))
            phaseChanger.StartChange(this);

        else if (collider.gameObject.TryGetComponent(out CarnivorousPlant plant) && collider == plant.headCollider)
            plant.Bite(this);

        else if (collider.gameObject.CompareTag("Roots"))
            Hurt();

        else if (collider.gameObject.layer == LayerMask.NameToLayer("EthanolFire"))
            Hurt();
        

    }
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Door")){
            moveAction.Disable();
            jumpAction.Disable();
            movementInput = Vector2.right;
            gameManager.LevelComplete();
        }

    }

    private void FixedUpdate()
    {
        movementInput = moveAction.ReadValue<Vector2>();


        hurtTimeoutRemaining -= Time.fixedDeltaTime;

        if (groundTilemap.GetTile(GridPosition) == gameManager.fireTile) {
            Hurt();
        }

        rb.drag = ModeData.drag * SizeData.dragMultiplier;

        if (movementInput.x < 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;

        // jump
        if (ModeData.doesJump && jumpAction.IsPressed() && IsOnGround()) {
            rb.velocityY = jumpVelocity;
        }

        if (CurrentMode == ModesEnum.Liquid) {

            rb.velocityX = movementInput.x * movementSpeed;

        }
        else if (CurrentMode == ModesEnum.Ice) {

            if (movementInput.x != 0)
                rb.velocityX = movementInput.x * movementSpeed;

        }
        else if (CurrentMode == ModesEnum.Cloud || CurrentMode == ModesEnum.Liquid_Underground) {

            if (movementInput.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movementInput.x * movementSpeed;

            if (movementInput.y != 0)
                rb.velocityY = movementInput.y * movementSpeed;

        }

        if ((CurrentMode == ModesEnum.Liquid || CurrentMode == ModesEnum.Liquid_Underground)
                && movementInput.y < 0 && groundCheck.CheckGround(groundLayers) && IsPorous(GridPosition + Vector3Int.down)) {
            CurrentMode = ModesEnum.Liquid_Underground;
        }
        else if ((CurrentMode == ModesEnum.Liquid_Underground) && groundTilemap.GetTile(GridPosition) == null) {
            CurrentMode = ModesEnum.Liquid;
        }

    }
}
