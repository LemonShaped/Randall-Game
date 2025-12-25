using UnityEngine;

public class PlayerController : LiquidCharacter
{

    public float hurtTimeout;
    public float hurtTimeoutRemaining;

    public Controls.PlayerMovementActions Actions => gameManager.controls.PlayerMovement;

    public Vector2 movement;

    [Tooltip("Stores held items.\nShould be an empty object that is a child of the player.")]
    public Transform inventory;


    public void EnableInput()
    {
        Actions.Enable();
    }
    public void DisableInput()
    {
        Actions.Disable();
        movement = Vector2.zero;
    }

    public override bool Hurt()
    {
        if (hurtTimeoutRemaining <= 0) {
            hurtTimeoutRemaining = hurtTimeout;
            return AddHealth(-1);
        }
        return false;
    }

    public override void Die()
    {
        gameManager.LevelFailed();
        Destroy(gameObject);
    }



    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();

    public override void Awake()
    {
        if (inventory == null) {
            Debug.LogWarning("PlayerController: inventory not set, using self as inventory by default");
            inventory = transform;
        }
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D collider)
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

    private void OnTriggerStay2D(Collider2D collider)
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

    private void OnTriggerExit2D(Collider2D collider)
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
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movement.x > 0 && assets[(int)CurrentMode].flippable)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;

        if (ModeData.doesJump && Actions.Jump.IsPressed() && IsOnGround()) {
            Jump();
        }

        if (CurrentMode == ModesEnum.Liquid || CurrentMode == ModesEnum.Jelly)
            rb.linearVelocityX = movement.x * movementSpeed;

        else if (CurrentMode == ModesEnum.Ice) {
            if (movement.x != 0)
                rb.linearVelocityX = movement.x * movementSpeed;
        }

        else if (CurrentMode == ModesEnum.Cloud || CurrentMode == ModesEnum.Liquid_Underground) {
            if (movement.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.linearVelocityX = movement.x * movementSpeed;
            if (movement.y != 0)
                rb.linearVelocityY = movement.y * movementSpeed;
        }


        if ((CurrentMode == ModesEnum.Liquid || CurrentMode == ModesEnum.Liquid_Underground || CurrentMode == ModesEnum.Jelly)
                && Actions.Down.IsPressed() && groundCheck.CheckGround(GroundLayers) && gameManager.IsPorousGround((Vector2)transform.position + Vector2.down)) {
            foreach (var item in inventory.GetComponentsInChildren<PickupObject>()) {
                item.Drop(this);
            }
            if (CurrentMode != ModesEnum.Liquid_Underground)
                CurrentMode = ModesEnum.Liquid_Underground;
        }

        else if ((CurrentMode == ModesEnum.Liquid_Underground) && gameManager.GetMaterial((Vector2)transform.position) == GroundMaterial.None)
            CurrentMode = ModesEnum.Liquid;

    }
}
