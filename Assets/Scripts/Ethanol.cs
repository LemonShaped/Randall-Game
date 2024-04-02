using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ethanol : LiquidCharacter
{

    public bool burning;

    [Tooltip("Time in seconds between losing 1 health")]
    public float burnTime;

    [Range(-1,1)]
    public int moveDirection;

    [Range(0,1)]
    public float jumpChance;

    [Tooltip("Trigger colliders outlining the flames around ethanol")]
    public Collider2D[] flameColliders;

    public TileBase[] noFiresOnTiles;


    private new void Start() {
        base.Start();
        StartCoroutine(BurnUp());
    }

    void FixedUpdate()
    {
        TileBase fireTile = gameManager.fireTile;

        if (groundTilemap.GetTile(GridPosition + (Vector3Int.right * moveDirection)) != null
                && groundTilemap.GetTile(GridPosition + (Vector3Int.right * moveDirection)) != fireTile) {

            if (Random.value > jumpChance) {
                moveDirection = -moveDirection;
                spriteRenderer.flipX = moveDirection < 0;
            }
            else if (IsOnGround())
                rb.velocityY = jumpVelocity;
                
        }

        bool doPlaceFire() {
            if (groundCheck.CheckGround(groundLayers) && groundTilemap.GetTile(GridPosition) == null) {
                foreach (var tile in noFiresOnTiles)
                    if (groundTilemap.GetTile(GridPosition + Vector3Int.down) == tile)
                        return false;
                return true;
            }
            return false;
        }

        if (doPlaceFire())
            gameManager.SetTile(GridPosition, fireTile);

        rb.velocityX = movementSpeed * moveDirection;


    }

    IEnumerator BurnUp()
    {
        while (true) {
            yield return new WaitForSeconds(burnTime);
            if (burning)
                Hurt();
        }
    }

    public override void UpdateTexture() {

        for (int size = 0; size < flameColliders.Length; size++) {
            flameColliders[size].enabled = (size == CurrentSize);
        }

        base.UpdateTexture();
    }

}
