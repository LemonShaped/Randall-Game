using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(MyAnimator)), RequireComponent(typeof(GroundCheck))]
public class LiquidCharacter : MonoBehaviour
{

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public MyAnimator animator;
    [HideInInspector] public GroundCheck groundCheck;

    public Tilemap groundTilemap;
    public LayerMask groundLayers;

    public float iceAndCloudTimeout;

    [NonReorderable]
    public MovementMode[] modes = new MovementMode[4];

    [NonReorderable]
    public PlayerSize[] sizes = new PlayerSize[5];


    public ModesEnum _currentMode = ModesEnum.Water;

    [Range(0, 4)]
    public int _currentSize;


    public MovementMode ModeData {
        get => modes[(int)CurrentMode];
    }
    public PlayerSize SizeData {
        get => sizes[CurrentSize];
    }


    [NonReorderable]
    public ModeAssets[] assets = new ModeAssets[4];

    public Vector3Int GridPosition {
        get => Vector3Int.FloorToInt(transform.position);
    }


    [HideInInspector]
    public float movementSpeed; // calculated based on size and movement mode
    [HideInInspector]
    public float jumpVelocity; //  ''


    private Coroutine liquification; // convert back to water after timeout

    /*[HideInInspector]*/ public bool canPickUpPuddles;
    /*[HideInInspector]*/ public bool canInteractWithStateChangerObjects;
    /*[HideInInspector]*/ public bool isHurtByEthanol;
    public float baseMovementSpeed = 1;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<MyAnimator>();
        groundCheck = GetComponent<GroundCheck>();

    }

    public void Start()
    {
        UpdateValues();
        UpdateTexture();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Spike")
            Hurt();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Puddle") && canPickUpPuddles) {
            if (AddHealth(1) == true)
                Destroy(collider.gameObject);
        }
        else if (collider.gameObject.TryGetComponent(out PhaseChangingObject phaseChanger) && canInteractWithStateChangerObjects)
            phaseChanger.StartChange(this);
        else if (collider.gameObject.TryGetComponent(out Ethanol _) && isHurtByEthanol)
            Hurt();

    }

    public bool IsOnGround() {
        return rb.IsTouchingLayers(groundLayers) && groundCheck.CheckGround(groundLayers);
    }

    public virtual bool Hurt() => AddHealth(-1);
    public bool AddHealth(int amount) {
        if (CurrentSize == 4 && amount > 0) {
            return false;
        }
        else if (CurrentSize == 0 && amount < 0) {
            spriteRenderer.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Debug.Log("Dead");//// go to death screen
            Destroy(gameObject);
            return true;
        }
        CurrentSize += amount;
        return true;
    }


    public bool IsPorous(Vector3Int pos) => IsPorous(pos, groundTilemap);
    public bool IsPorous(Vector3Int pos, Tilemap tilemap)
    {
        TileData tileData = default;
        tilemap.GetTile(pos).GetTileData(pos, tilemap, ref tileData);

        return tileData.gameObject != null && tileData.gameObject.layer == LayerMask.NameToLayer("GroundPorous");
    }

    public ModesEnum CurrentMode {
        get => _currentMode;
        set {
            _currentMode = value;

            if (_currentMode == ModesEnum.Water_Underground)
                rb.excludeLayers |= (1 << LayerMask.NameToLayer("GroundPorous")); // exclude collisions with porous ground
            else
                rb.excludeLayers &= ~(1 << LayerMask.NameToLayer("GroundPorous")); // allow collisions with porous ground

            if (_currentMode == ModesEnum.Ice || _currentMode == ModesEnum.Cloud) {
                if (liquification is null)
                    liquification = StartCoroutine(DelayedConvert(ModesEnum.Water, iceAndCloudTimeout));
            }
            else if (liquification is not null)
                StopCoroutine(liquification);


            UpdateValues();
            UpdateTexture();
        }
    }
    public int CurrentSize {
        get => _currentSize;
        set {
            _currentSize = value;
            UpdateValues();
            UpdateTexture();
        }
    }

    public IEnumerator DelayedConvert(ModesEnum convertInto, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        CurrentMode = convertInto;
        liquification = null;
    }

    public void UpdateValues()
    {
        movementSpeed = baseMovementSpeed * ModeData.speed * SizeData.speedMultiplier;
        jumpVelocity = 0;

        if (ModeData.doesJump) {
            float jumpHeight = ModeData._jumpHeight * SizeData._jumpHeightMultiplier;
            float jumpDuration = ModeData._jumpDuration * SizeData._jumpDurationMultiplier;

            jumpVelocity = 2 * jumpHeight / jumpDuration;
            rb.gravityScale = (jumpVelocity / jumpDuration) / -Physics2D.gravity.y;
            rb.drag = ModeData.drag * SizeData.dragMultiplier;
        }
        else {
            rb.gravityScale = ModeData.gravityScale * SizeData.gravityScaleMultiplier;
            rb.drag = ModeData.drag * SizeData.dragMultiplier;

        }
    }

    public void UpdateTexture()
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
            collider.enabled = (collider == assets[(int)CurrentMode].sizes[CurrentSize].collider);
        groundCheck.pointA = assets[(int)CurrentMode].sizes[CurrentSize].groundCheck_A;
        groundCheck.pointB = assets[(int)CurrentMode].sizes[CurrentSize].groundCheck_B;

        animator.StartAnimation(assets[(int)CurrentMode].sizes[CurrentSize].sprites);
        
    }


#if UNITY_EDITOR
    private void OnValidate()
    {

        if (modes.Length != Enum.GetValues(typeof(ModesEnum)).Length || assets.Length != Enum.GetValues(typeof(ModesEnum)).Length || sizes.Length > 5) {
            Array.Resize(ref modes, Enum.GetValues(typeof(ModesEnum)).Length);
            Array.Resize(ref assets, Enum.GetValues(typeof(ModesEnum)).Length);
            Array.Resize(ref sizes, 5);
            Debug.LogError("Incorrect array length!");
        }
        foreach (ModeAssets modeTextures in assets) {
            if (modeTextures.sizes.Length != 5) {
                Debug.LogError("Incorrect array length!");
                Array.Resize(ref modeTextures.sizes, 5);
            }
            foreach (ModeAssets.SizeAssets size in modeTextures.sizes) {
                if (size.sprites.Length < 1) {
                    Debug.LogError("Array must have >= 1 item, even if texture is empty.");
                    Array.Resize(ref size.sprites, 1);
                }
            }
        }


        for (int i = 0; i < modes.Length; i++)
            modes[i]._name = Enum.GetName(typeof(ModesEnum), i);

        for (int i = 0; i < sizes.Length; i++)
            sizes[i]._name = $"Size {i}";

        for (int i = 0; i < assets.Length; i++) {
            assets[i]._name = Enum.GetName(typeof(ModesEnum), i);

            for (int j = 0; j < assets[i].sizes.Length; j++) {
                assets[i].sizes[j]._name = $"Size {j}";
            }
        }

        CurrentMode = _currentMode;
        CurrentSize = _currentSize;
    }
#endif

}


public enum ModesEnum
{
    Water, Water_Underground, Ice, Cloud
}

[Serializable]
public class MovementMode
{
    [HideInInspector]
    public string _name;

    [Tooltip("Movement speed in blocks/second")]
    public float speed;

    [Tooltip("Air resistance")]
    public float drag = 0;

    [Tooltip("Multiplier applied to gravity. Value only used if mode does not jump")]
    public float gravityScale = 1;

    public bool doesJump = false;

    [Tooltip("Max height of jump")]
    public float _jumpHeight;
    [Tooltip("Time until peak of jump")]
    public float _jumpDuration;
}
[Serializable]
public class PlayerSize
{
    [HideInInspector]
    public string _name;

    public float speedMultiplier = 1;

    public float dragMultiplier = 1;

    [Tooltip("Value only used for Modes that do not jump")]
    public float gravityScaleMultiplier = 1;

    public float _jumpHeightMultiplier = 1;
    public float _jumpDurationMultiplier = 1;

}



[Serializable]
public class ModeAssets
{
    [HideInInspector]
    public string _name;

    public bool flippable;
    [NonReorderable]
    public SizeAssets[] sizes = new SizeAssets[5];

    [Serializable]
    public class SizeAssets
    {
        [HideInInspector]
        public string _name;

        public Collider2D collider;
        public Vector2 groundCheck_A;
        public Vector2 groundCheck_B;

        [Tooltip("Sprite(s) to use for this size. Multiple for idle animation")]
        public Sprite[] sprites = new Sprite[1];
    }

}
