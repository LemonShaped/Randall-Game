using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[Serializable]
public class MovementMode
{
    [SerializeField] public string name;

    [SerializeField]
    [InspectorLabel("(jump height)")]
    private float _jumpHeight;
    [SerializeField]
    [InspectorLabel("(jump time)")]
    private float _jumpTime;
    public float jumpVelocity;
    public float gravityScale;

    public float drag;
    public float speed;

    public void SetupJump() {
        if (_jumpHeight == 0 || _jumpTime == 0)
            return;

        jumpVelocity = 2 * _jumpHeight / _jumpTime;
        gravityScale = (jumpVelocity / _jumpTime) / -Physics2D.gravity.y;
    }

}

public class PlayerController : MonoBehaviour
{
    public enum ModesEnum {
        Water, Underground, Ice, Cloud
    }
    [Serializable]
    public class Modes
    {
        public MovementMode Water;
        public MovementMode Underground;
        public MovementMode Ice;
        public MovementMode Cloud;
        private Modes() { }
        private static Modes _instance;
        public static Modes Instance => _instance ??= new Modes();
    }

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

    public Modes modes = Modes.Instance;

    public ModesEnum currentMode;
    public MovementMode CurrentMode {
        get => modes.GetType().GetField(currentMode.ToString()).GetValue(modes) as MovementMode;
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
        modes.Water.SetupJump();
        modes.Underground.SetupJump();
        modes.Ice.SetupJump();
        modes.Cloud.SetupJump();
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
