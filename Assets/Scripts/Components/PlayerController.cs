using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LiquidCharacter
{

    public float hurtTimeout;
    public float hurtTimeoutRemaining;

    Controls.PlayerMovementActions Actions => gameManager.controls.PlayerMovement;

    public Vector2 movement;

    [Tooltip("Stores held items.\nShould be an empty object that is a child of the player.")]
    public Transform inventoryObj;

    public List<PickupObject> inventory = new();

    void EnableInput()
    {
        Actions.Enable();
    }

    void DisableInput()
    {
        Actions.Disable();
        movement = Vector2.zero;
    }

    public override bool Hurt()
    {
        if (hurtTimeoutRemaining > 0)
            return false;
        hurtTimeoutRemaining = hurtTimeout;
        return AddHealth(-1);
    }

    public override void Die()
    {
        gameManager.LevelFailed();
        Destroy(gameObject);
    }



    void OnEnable() => EnableInput();
    void OnDisable() => DisableInput();

    public override void Awake()
    {
        if (inventoryObj == null) {
            Debug.LogWarning("PlayerController: inventory not set, using self as inventory by default");
            inventoryObj = transform;
        }
        base.Awake();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.TryGetComponent(out StateChangingObject stateChanger))
            stateChanger.WaitingToStart(this);

        else if (collider.gameObject.CompareTag("Door")) {
            hurtTimeoutRemaining = 3000;
            DisableInput();
            movement = Vector2.right;
            gameManager.LevelComplete();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
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
        else if (collider.gameObject.CompareTag("PickupObject")) {
            PickupObject pickupObject = collider.gameObject.GetComponent<PickupObject>();
            pickupObject.PickUp(this);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out StateChangingObject stateChanger))
            stateChanger.CancelBeforeStarted(this);
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Actions.Move2D.enabled)
            movement = Actions.Move2D.ReadValue<Vector2>();
        else if (Actions.Sideways.enabled)
            movement = new Vector2(Actions.Sideways.ReadValue<float>(), 0);

        if (hurtTimeoutRemaining > 0)
            hurtTimeoutRemaining -= Time.fixedDeltaTime;


        if (movement.x < 0 && assets[(int)CurrentMode].flippable)
            spriteRenderer.flipX = true;
        else if (movement.x > 0 && assets[(int)CurrentMode].flippable)
            spriteRenderer.flipX = false;

        if (ModeData.doesJump && Actions.Jump.IsPressed() && IsOnGround()) {
            Jump();
        }

        switch (CurrentMode)
        {
            case ModesEnum.Liquid:
            case ModesEnum.Jelly:
                rb.linearVelocityX = movement.x * movementSpeed;
                break;

            case ModesEnum.Ice:
                if (movement.x != 0)
                    rb.linearVelocityX = movement.x * movementSpeed;
                break;

            case ModesEnum.Cloud:
            case ModesEnum.LiquidUnderground:
                if (movement.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                    rb.linearVelocityX = movement.x * movementSpeed;
                if (movement.y != 0)
                    rb.linearVelocityY = movement.y * movementSpeed;
                break;
        }


        bool onPorousGround = groundCheck.CheckGround(GroundLayers)
                              && gameManager.IsPorousGround((Vector2)transform.position + Vector2.down);

        // Liquid -> to Liquid_Underground
        if (CurrentMode is ModesEnum.Liquid or ModesEnum.LiquidUnderground or ModesEnum.Jelly
            && Actions.Down.IsPressed() && onPorousGround)
        {
            foreach (PickupObject item in inventoryObj.GetComponentsInChildren<PickupObject>()) {
                item.Drop(this);
            }
            if (CurrentMode != ModesEnum.LiquidUnderground)
                CurrentMode = ModesEnum.LiquidUnderground;
        }
        // Liquid_Underground -> Liquid
        else if (CurrentMode == ModesEnum.LiquidUnderground
                 && gameManager.GetMaterial((Vector2)transform.position) == GroundMaterial.None)
            CurrentMode = ModesEnum.Liquid;

    }
}
