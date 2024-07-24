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

    [Tooltip("Trigger-colliders that outline the flames around ethanol")]
    public Collider2D[] flameColliders;

    [Tooltip("Fires will not be placed above these blocks. Do include null to avoid fires in the air.")]
    public GroundMaterial[] noFiresAbove;

    public override void Start() {
        base.Start();
        StartCoroutine(BurnUp());
    }

    void FixedUpdate()
    {

        if (gameManager.GetMaterial((Vector2)transform.position + (Vector2.right * moveDirection)) != GroundMaterial.None) {

            if (Random.value <= jumpChance) {
                if (IsOnGround())
                    rb.velocityY = jumpVelocity;
            }
            else {
                moveDirection = -moveDirection;
                spriteRenderer.flipX = moveDirection < 0;
            }

        }

        bool doPlaceFire() {
            if (groundCheck.CheckGround(groundLayers) && gameManager.GetMaterial((Vector2)transform.position) == GroundMaterial.None) {
                foreach (var material in noFiresAbove)
                    if (gameManager.GetMaterial((Vector2)transform.position + Vector2.down) == material)
                        return false;
                return true;
            }
            return false;
        }

        if (doPlaceFire())
            gameManager.PlaceFire(Vector3Int.FloorToInt(transform.position));

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
