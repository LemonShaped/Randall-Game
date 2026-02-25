using System.Collections;
using System.Linq;
using UnityEngine;

public class Ethanol : LiquidCharacter
{

    public bool burning;

    [Tooltip("Time in seconds between losing 1 health")]
    public float burnTime;

    [Range(-1, 1)]
    public int moveDirection;

    [Range(0, 1)]
    public float jumpChance;

    [Tooltip("Trigger-colliders that outline the flames around ethanol")]
    public Collider2D[] flameColliders;

    [Tooltip("Fires will not be placed above these blocks. Do include null to avoid fires in the air.")]
    public GroundMaterial[] noFiresAbove;

    public override void Start()
    {
        base.Start();
        StartCoroutine(BurnUp());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (gameManager.GetMaterial((Vector2)transform.position + (Vector2.right * moveDirection)) != GroundMaterial.None) {

            if (Random.value <= jumpChance) {
                if (IsOnGround())
                    rb.linearVelocityY = jumpVelocity;
            }
            else {
                moveDirection = -moveDirection;
                spriteRenderer.flipX = moveDirection < 0;
            }

        }

        rb.linearVelocityX = movementSpeed * moveDirection;

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GroundLayers.Contains(collision.gameObject.layer)
            || noFiresAbove.Contains(gameManager.GetMaterial(collision.gameObject.layer)))
            return;

        for (int i = 0; i < collision.contactCount; i++) {
            float angle = Vector2.SignedAngle(Vector2.up, collision.GetContact(i).normal);
            if (-60f < angle && angle < 60f) {
                gameManager.PlaceFire(collision.GetContact(i).point, angle);
            }
        }
    }

    IEnumerator BurnUp()
    {
        while (true) {
            yield return new WaitForSeconds(burnTime);
            if (burning)
                Hurt();
        }
    }

    public override void UpdateTexture()
    {

        for (int size = 0; size < flameColliders.Length; size++) {
            flameColliders[size].enabled = size == CurrentSize;
        }

        base.UpdateTexture();
    }

}
