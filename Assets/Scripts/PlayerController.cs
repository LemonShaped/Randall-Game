using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : LiquidCharacter
{

    public float hurtTimeout;
    public float hurtTimeoutRemaining;

    public InputAction moveAction;
    public InputAction jumpAction;

    public Vector2 movementInput;

    private void OnEnable() {
        moveAction.Enable();
        jumpAction.Enable();
    }
    private void OnDisable() {
        moveAction.Disable();
        jumpAction.Disable();
    }


    public override bool Hurt() {
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


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Spike"))
            Hurt();
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Puddle")) {
            bool healthChanged = AddHealth(1);
            if (healthChanged)
                Destroy(collider.gameObject); // only delete the puddle if it actually increased our health (not if we are already at max)
        }

        else if (collider.gameObject.TryGetComponent(out CarnivorousPlant plant) && collider == plant.headCollider)
            plant.Bite(this);

        else if (collider.gameObject.CompareTag("Roots"))
            Hurt();

        else if (collider.gameObject.layer == LayerMask.NameToLayer("EthanolFire"))
            Hurt();

        else if (collider.gameObject.CompareTag("Fire"))
            Hurt();
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.gameObject.TryGetComponent(out StateChangingObject stateChanger))
            stateChanger.WaitingToStart(this);

        else if (collider.gameObject.CompareTag("Door")) {
            hurtTimeoutRemaining = 1000;
            moveAction.Disable();
            jumpAction.Disable();
            movementInput = Vector2.right;
            gameManager.LevelComplete();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.gameObject.TryGetComponent(out StateChangingObject stateChanger))
            stateChanger.CancelBeforeStarted(this);
    }


    private void FixedUpdate() {
        if (moveAction.enabled)
            movementInput = moveAction.ReadValue<Vector2>();

        if (hurtTimeoutRemaining > 0)
            hurtTimeoutRemaining -= Time.fixedDeltaTime;


        if (movementInput.x < 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;

        if (ModeData.doesJump && jumpAction.IsPressed() && IsOnGround()) {
            Jump();
        }

        if (CurrentMode == ModesEnum.Liquid)
            rb.velocityX = movementInput.x * movementSpeed;

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
                && movementInput.y < 0 && groundCheck.CheckGround(groundLayers) && gameManager.IsPorousGround((Vector2)transform.position + Vector2.down)) {
            if (CurrentMode != ModesEnum.Liquid_Underground)
                CurrentMode = ModesEnum.Liquid_Underground;
        }

        else if ((CurrentMode == ModesEnum.Liquid_Underground) && gameManager.GetMaterial((Vector2)transform.position) == GroundMaterial.None)
            CurrentMode = ModesEnum.Liquid;

    }
}
