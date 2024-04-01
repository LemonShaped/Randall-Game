using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ethanol : LiquidCharacter
{

    public PlayerController player;
    //public bool takeValuesFromPlayer;

    public bool burning;

    [Tooltip("Time in seconds between losing 1 health")]
    public float burnTime;

    [Range(-1,1)]
    public int moveDirection;

    [Range(0,1)]
    public float jumpChance;

    [Tooltip("Trigger colliders outlining the flames around ethanol")]
    public Collider2D[] flameColliders;

    public TileBase fireTile;

    private new void Awake()
    {
        base.Awake();

        groundTilemap = GameObject.FindWithTag("Ground Tilemap").GetComponent<Tilemap>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        canPickUpPuddles = false;
        canInteractWithStateChangerObjects = false;
        isHurtByEthanol = false;
    }

    private new void Start() {
        base.Start();
        StartCoroutine(BurnUp());
    }

    void FixedUpdate()
    {

        if (groundTilemap.GetTile(GridPosition + (Vector3Int.right * moveDirection))) {

            if (Random.value > jumpChance) {
                moveDirection = -moveDirection;
                spriteRenderer.flipX = moveDirection < 0;
            }
            else if (IsOnGround())
                rb.velocityY = jumpVelocity;
                
        }
        if (groundTilemap.GetTile(GridPosition) is null &&
                groundTilemap.GetTile(GridPosition + Vector3Int.down) is not null && groundCheck.CheckGround(groundLayers)){
            groundTilemap.SetTile(GridPosition, fireTile);
        }

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

}
