using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{

    public InputAction moveAction;
    public InputAction jumpAction;

    public Rigidbody2D rb;

    public GroundCheck groundCheck;

    public Tilemap groundTilemap;
    public LayerMask groundLayers;

    public enum ModesEnum {
        Water, Water_Underground, Ice, Cloud
    }

    [NonReorderable]
    public MovementMode[] modes = new MovementMode[4];

    [NonReorderable]
    public PlayerSize[] sizes = new PlayerSize[5];

    public ModesEnum currentMode = ModesEnum.Water;

    [Range(0, 4)]
    public int currentSize;

    public MovementMode ModeData {
        get => modes[(int)currentMode];
    }
    public PlayerSize SizeData {
        get => sizes[currentSize];
    }
    public Vector3Int PlayerTile {
        get => Vector3Int.FloorToInt(transform.position);
    }


    private float speed; // calculated based on size and movement mode
    private float jumpVelocity; //  ''
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
        if (modes.Length != Enum.GetValues(typeof(ModesEnum)).Length) {
            Array.Resize(ref modes, Enum.GetValues(typeof(ModesEnum)).Length);
            Debug.LogError("No changing array size!!!");
        }

        foreach (MovementMode mode in modes) {
            mode.name = Enum.GetName(typeof(ModesEnum), i);
        }
    }

    private void FixedUpdate()
    {

        Vector2 movementInput = moveAction.ReadValue<Vector2>();

        if (movementInput.x < 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;


        if (ModeData.doesJump) {

            rb.velocityX = movementInput.x * speed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = jumpVelocity;
            }

        }
        else {

            if (movementInput.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movementInput.x * speed;

            if (movementInput.y != 0)
                rb.velocityY = movementInput.y * speed;

        }

        if ((currentMode == ModesEnum.Water || currentMode == ModesEnum.Water_Underground)
                && movementInput.y < 0 && groundCheck.CheckGround(groundLayers) && IsTilePorous(groundTilemap, PlayerTile + Vector3Int.down)) {
            currentMode = ModesEnum.Water_Underground;
            rb.excludeLayers |= (1 << LayerMask.NameToLayer("GroundPorous")); // exclude collisions with porous ground
        }
        else if ((currentMode == ModesEnum.Water_Underground)
                && groundTilemap.GetTile(PlayerTile) == null) {
            currentMode = ModesEnum.Water;
            rb.excludeLayers &= ~(1 << LayerMask.NameToLayer("GroundPorous")); // allow collisions with porous ground
        }
    }


    private bool IsOnGround() {
        return rb.IsTouchingLayers(groundLayers) && groundCheck.CheckGround(groundLayers);
    }

    private void Hurt() => AddHealth(-1);
    private bool AddHealth(int amount) {
        SetCurrentSize(currentSize + amount);
        if (currentSize < 0) {
            SetCurrentSize(0);
            Debug.Log("Dead");//// go to death screen
        }
        else if (currentSize > 4) {
            SetCurrentSize(4);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Spike") {
            Hurt();
        }
    }
    private bool IsPorous(Vector3Int pos, Tilemap tilemap)
    {
        TileData tileData = default;
        tilemap.GetTile(pos).GetTileData(pos, tilemap, ref tileData);

        return tileData.gameObject != null && tileData.gameObject.layer == LayerMask.NameToLayer("GroundPorous");
        

    public void SetCurrentSize(int size)
    {
        currentSize = size;

        speed = ModeData.speed * SizeData.speedMultiplier;
        jumpVelocity = 0;

        if (ModeData.doesJump) {
            float jumpHeight = ModeData._jumpHeight * SizeData._jumpHeightMultiplier;
            float jumpDuration = ModeData._jumpDuration * SizeData._jumpDurationMultiplier;

            jumpVelocity = 2 * jumpHeight / jumpDuration;
            rb.gravityScale = (jumpVelocity / jumpDuration) / -Physics2D.gravity.y;
        }
        else {
            rb.gravityScale = ModeData.gravityScale * SizeData.gravityScaleMultiplier;
            rb.drag = ModeData.drag * SizeData.dragMultiplier;
            
        }
    }

}


[Serializable]
public class MovementMode
{
    [HideInInspector]
    public string name;

    [Tooltip("Movement speed in blocks/second")]
    public float speed;

    [Tooltip("Air resistance")]
    public float drag;

    [Tooltip("Multiplier applied to gravity. If doesJump, this is ignored and is calculated automatically")]
    public float gravityScale;

    public bool doesJump = false;

    [Tooltip("Max height of jump")]
    public float _jumpHeight;
    [Tooltip("Time until peak of jump")]
    public float _jumpDuration;

}


