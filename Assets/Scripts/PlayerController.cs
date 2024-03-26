using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[Serializable]
public class MovementMode
{
    [HideInInspector] public string name;

    [SerializeField, Tooltip("Max height of jump")]
    private float _jumpHeight;

    [SerializeField, Tooltip("Time until peak of jump")]
    private float _jumpDuration;

    public float jumpVelocity;

    [Tooltip("Multiplier applied to gravity")]
    public float gravityScale;

    [Tooltip("Air resistance")]
    public float drag;

    [Tooltip("Movement speed in blocks/second")]
    public float speed;

    public void SetupJump() {
        if (_jumpHeight != 0 && _jumpDuration != 0) {
            jumpVelocity = 2 * _jumpHeight / _jumpDuration;
            gravityScale = (jumpVelocity / _jumpDuration) / -Physics2D.gravity.y;
        }
    }

}

public class PlayerController : MonoBehaviour
{

    public InputAction moveAction;
    public InputAction jumpAction;
    public Vector2 movementInput;

    public Rigidbody2D rb;

    public LayerMask groundLayers;
    public Tilemap groundTilemap;
    public GroundCheck groundCheck;

    [Range(0,5)]
    public int health = 5;

    public enum ModesEnum {
        Water, Water_Underground, Ice, Cloud
    }

    [NonReorderable]
    public MovementMode[] modes = new MovementMode[4];

    public ModesEnum currentMode;
    public MovementMode CurrentMode {
        get => modes[(int)currentMode];
    }
    public Vector3Int PlayerTile {
        get => Vector3Int.FloorToInt(transform.position);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        OnValidate();
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
            //mode.name = Enum.GetName(typeof(ModesEnum), i);
            mode.SetupJump();
        }
    }

    private void FixedUpdate()
    {
        movementInput = moveAction.ReadValue<Vector2>();

        rb.gravityScale = CurrentMode.gravityScale;
        rb.drag = CurrentMode.drag;

        if (movementInput.x < 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;


        if (currentMode == ModesEnum.Cloud || currentMode == ModesEnum.Water_Underground) {

            if (movementInput.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movementInput.x * CurrentMode.speed;

            if (movementInput.y != 0)
                rb.velocityY = movementInput.y * CurrentMode.speed;

        }
        else {

            rb.velocityX = movementInput.x * CurrentMode.speed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = CurrentMode.jumpVelocity;
            }

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

    private void Hurt() => AddHealth(-1);

    private void AddHealth(int amount) {
        health += amount;
        if (health < 0) {
            health = 0;
            Debug.Log("Dead");//// go to death screen
        }
        else if (health > 5) {
            health = 5;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Spike") {
            Hurt();
        }
    }

    private bool IsTilePorous(Tilemap tilemap, Vector3Int pos)
    {
        TileData tileData = default;
        tilemap.GetTile(pos).GetTileData(pos, tilemap, ref tileData);

        return tileData.gameObject.layer == LayerMask.NameToLayer("GroundPorous");
        
    }

    private bool IsOnGround()
    {
        return rb.IsTouchingLayers(groundLayers) && groundCheck.CheckGround(groundLayers);
    }


}
