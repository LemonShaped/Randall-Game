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
    public Vector2 movement;

    public Rigidbody2D rb;

    public LayerMask groundLayers;
    public Tilemap groundTilemap;
    public GroundCheck groundCheck;

    public Vector3Int PlayerTile {
        get => Vector3Int.FloorToInt(transform.position);
    }

    public enum ModesEnum {
        Water, Underground, Cloud, Ice
    }

    [NonReorderable]
    public MovementMode[] modes = new MovementMode[4];

    public ModesEnum currentMode;
    public MovementMode CurrentMode {
        get => modes[(int)currentMode];
    }

    public TileBase[] porousTiles;

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

        foreach (int i in Enum.GetValues(typeof(ModesEnum))) {
            modes[i].name = Enum.GetName(typeof(ModesEnum), i);
            modes[i].SetupJump();
        }
    }

    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (currentMode == ModesEnum.Cloud || currentMode == ModesEnum.Underground) {
            rb.gravityScale = CurrentMode.gravityScale;
            rb.drag = CurrentMode.drag;

            if (movement.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movement.x * CurrentMode.speed;

            if (movement.y != 0)
                rb.velocityY = movement.y * CurrentMode.speed;

        }
        else if (currentMode == ModesEnum.Water){
            rb.gravityScale = CurrentMode.gravityScale;
            rb.drag = CurrentMode.drag;

            rb.velocityX = movement.x * CurrentMode.speed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = CurrentMode.jumpVelocity;
            }

        }

        if (movement.y < 0 && groundCheck.CheckGround(groundLayers) && IsTilePorous(groundTilemap.GetTile(PlayerTile + Vector3Int.down))) {
            currentMode = ModesEnum.Underground;
            rb.excludeLayers |= (1 << LayerMask.NameToLayer("GroundPorous")); // exclude collisions with porous ground
        }
        else if (groundTilemap.GetTile(PlayerTile) == null) {
            currentMode = ModesEnum.Water;
            rb.excludeLayers &= ~(1 << LayerMask.NameToLayer("GroundPorous")); // allow collisions with porous ground
        }
    }

    private bool IsTilePorous(TileBase tile)
    {
        //if (tile.GetType() == typeof(Tile)) {
        //    Tile tile_ = (Tile)tile;
        //    return LayerMask.LayerToName(tile_.gameObject.layer) == "GroundPorous";
        //}
        //else if (tile.GetType() == typeof(RuleTile)) {
        //    RuleTile tile_ = (RuleTile)tile;
        //    return LayerMask.LayerToName(tile_.m_DefaultGameObject.layer) == "GroundPorous";
        //}
        foreach (TileBase porousTile in porousTiles) {
            if (tile == porousTile)
                return true;
        }
        return false;
    }

    private bool IsOnGround()
    {
        return rb.IsTouchingLayers(groundLayers) && groundCheck.CheckGround(groundLayers);
    }


}
