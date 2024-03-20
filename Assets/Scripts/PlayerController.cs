using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[Serializable]
public class MovementMode
{
    [SerializeField] public readonly string name;

    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpTime;
    public float jumpVelocity;
    public float gravityScale;

    public float drag;
    public float speed;

    public void SetupJump() {
        jumpVelocity = 2 * _jumpHeight / _jumpTime;
        gravityScale = (jumpVelocity / _jumpTime) / -Physics2D.gravity.y;
    }

    public MovementMode(string name) {
        this.name = name;
    }

    //public override bool Equals(object obj) =>
    //    obj is not null && obj.GetType() == typeof(MovementMode) && name == ((MovementMode)obj).name;
    //public static bool operator ==(MovementMode b1, MovementMode b2) =>
    //    b1 is not null && b1.Equals(b2);
    //public static bool operator !=(MovementMode b1, MovementMode b2) =>
    //    !(b1 == b2);

}

[Serializable]
public class Modes
{
    public MovementMode Water = new("Water");
    public MovementMode Underground = new("Underground");
    public MovementMode Ice = new("Ice");
    public MovementMode Cloud = new("Cloud");
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

    public Modes modes;
    public MovementMode currentMode;

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
        //foreach (MovementMode mode in modes) {
            //mode.SetupJump();
        //}

        //modes = new MovementMode[]{
        //    new MovementMode("Water", 3.1f, 0.9f),
        //    new MovementMode("Underground"){ gravityScale = 0.0f },
        //    new MovementMode("Ice", 1.05f, 0.5f),
        //    new MovementMode("Cloud"){ gravityScale = 0.1f }
        //};
    }

    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (currentMode == modes.Cloud || currentMode == modes.Underground) {
            rb.gravityScale = currentMode.gravityScale;
            rb.drag = currentMode.drag;

            if (movement.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movement.x * currentMode.speed;

            if (movement.y != 0)
                rb.velocityY = movement.y * currentMode.speed;

        }
        else if (currentMode == modes.Water){
            rb.gravityScale = currentMode.gravityScale;
            rb.drag = currentMode.drag;

            rb.velocityX = movement.x * currentMode.speed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = currentMode.jumpVelocity;
            }

        }

        if (movement.y < 0 && groundCheck.CheckGround(groundLayers) && IsTilePorous(groundTilemap.GetTile(PlayerTile + Vector3Int.down))) {
            currentMode = modes.Underground;
            rb.excludeLayers |= (1 << LayerMask.NameToLayer("GroundPorous")); // exclude collisions with porous ground
        }
        else if (groundTilemap.GetTile(PlayerTile) == null) {
            currentMode = modes.Water;
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
