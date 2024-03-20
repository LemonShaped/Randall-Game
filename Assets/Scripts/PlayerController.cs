using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MovementMode
{
    public string name;

    public float speed;
    
    public float jumpVelocity;
    public float gravityScale;
    public float drag;

    public void SetupJump(float jumpHeight, float jumpTime) {
        jumpVelocity = 2 * jumpHeight / jumpTime;
        gravityScale = (jumpVelocity / jumpTime) / -Physics2D.gravity.y;
    }
    
    public MovementMode(string name, float jumpHeight, float JumpTime) {
        this.name = name;
        SetupJump(jumpHeight, JumpTime);
    }
    public MovementMode(string name) {
        this.name = name;
    }
}//put jymo height and jukp speed ijto the class.

public class PlayerController : MonoBehaviour
{
    
    public InputAction moveAction;
    public InputAction jumpAction;
    public Vector2 movement;

    public Rigidbody2D rb;

    public LayerMask groundLayers;
    public Tilemap groundTilemap;
    public GroundCheck groundCheck;

    public Vector3Int playerTile {
        get => Vector3Int.FloorToInt(transform.position);
    }

    public enum Mode : int {
        Water,
        Underground,
        Ice,
        Cloud,
    }

    public MovementMode[] modes = {
        new MovementMode("Water", 3.1f, 0.9f),
        new MovementMode("Underground"){ gravityScale = 0.0f },
        new MovementMode("Ice", 1.05f, 0.5f),
        new MovementMode("Cloud"){ gravityScale = 0.1f },
    };
    public Mode currentMode = Mode.Water;
    public MovementMode CurrentMode { get => modes[(int)currentMode]; }

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
        foreach (MovementMode mode in modes) {
            mode.SetupJump();
        }
    }
    /* s = t(u+v)/2
     * u = 2s/t
     * jumpVelocity = 2*jumpHeight/jumpTime
     * 
     * v = u + at
     * a= -u/t
     * gravity = -jumpVelocity/jumpTime
     */

    private void FixedUpdate()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (doHover) {
            rb.gravityScale = hoverGravityScale;
            rb.drag = hoverDrag;

            if (movement.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movement.x * walkSpeed;

            if (movement.y != 0)
                rb.velocityY = movement.y * walkSpeed;

        }
        else {
            rb.gravityScale = normalGravityScale;
            rb.drag = 0;

            rb.velocityX = movement.x * walkSpeed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = jumpVelocity;
            }

        }

        if (movement.y < 0 && groundCheck.CheckGround(groundLayers) && IsTilePorous(groundTilemap.GetTile(playerTile + Vector3Int.down))) {
            doHover = true;
            rb.excludeLayers |= (1 << LayerMask.NameToLayer("GroundPorous")); // exclude collisions with porous ground
        }
        else if (groundTilemap.GetTile(playerTile) == null) {
            doHover = false;
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
