using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{

    public InputAction moveAction;
    public InputAction jumpAction;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    public GroundCheck groundCheck;

    public Tilemap groundTilemap;
    public LayerMask groundLayers;

    public enum ModesEnum {
        Water, Water_Underground, Ice, Cloud
    }


    [NonReorderable]
    public MovementMode[] modes = new MovementMode[4];

    [NonReorderable]
    public PlayerSize[] sizes = new PlayerSize[5];


    [SerializeField]
    private ModesEnum _currentMode = ModesEnum.Water;

    [SerializeField, Range(0, 4)]
    private int _currentSize;


    public MovementMode ModeData {
        get => modes[(int)CurrentMode];
    }
    public PlayerSize SizeData {
        get => sizes[CurrentSize];
    }


    [NonReorderable]
    public ModeTextures[] textures = new ModeTextures[4];

    [Tooltip("frames per second")]
    public float animationSpeed;

    private bool animating;

    public Vector3Int PlayerTile {
        get => Vector3Int.FloorToInt(transform.position);
    }


    private float speed; // calculated based on size and movement mode
    private float jumpVelocity; //  ''


    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        UpdateValues();
        UpdateTexture();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        animating = false;
    }

    private void FixedUpdate()
    {

        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        rb.drag = ModeData.drag * SizeData.dragMultiplier;

        if (movementInput.x < 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else if (movementInput.x > 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;


        if (ModeData.doesJump) {

            rb.velocityX = movementInput.x * speed;

            // jump
            if (jumpAction.IsPressed() && IsOnGround()) {
                rb.velocityY = jumpVelocity;
            }

        }
        else {

            if (movementInput.x != 0) // we want to control the speed directly but we dont want to stop instantly, when flying.
                rb.velocityX = movementInput.x * speed;

            if (movementInput.y != 0)
                rb.velocityY = movementInput.y * speed;

        }

        if ((CurrentMode == ModesEnum.Water || CurrentMode == ModesEnum.Water_Underground)
                && movementInput.y < 0 && groundCheck.CheckGround(groundLayers) && IsPorous(PlayerTile + Vector3Int.down)) {
            CurrentMode = ModesEnum.Water_Underground;
        }
        else if ((CurrentMode == ModesEnum.Water_Underground)
                && groundTilemap.GetTile(PlayerTile) == null) {
            CurrentMode = ModesEnum.Water;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) => Collision(collision.collider);
    private void OnTriggerEnter2D(Collider2D collider) => Collision(collider);
    private void Collision(Collider2D collider)
    {
        if (collider.gameObject.name == "Spike") {
            Hurt();
        }
        else if (collider.gameObject.tag == "Puddle") {
            AddHealth(1);
            Destroy(collider.gameObject);
        }
    }

    private bool IsOnGround() {
        return rb.IsTouchingLayers(groundLayers) && groundCheck.CheckGround(groundLayers);
    }

    private void Hurt() => AddHealth(-1);
    private bool AddHealth(int amount) {
        _currentSize += amount;
        if (_currentSize < 0) {
            CurrentSize = 0;
            Debug.Log("Dead");//// go to death screen
        }
        else if (_currentSize > 4) {
            CurrentSize = 4;
            return false;
        }
        CurrentSize = _currentSize;
        return true;
    }


    private bool IsPorous(Vector3Int pos) => IsPorous(pos, groundTilemap);
    private bool IsPorous(Vector3Int pos, Tilemap tilemap)
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

    public void UpdateValues()
    {
        speed = ModeData.speed * SizeData.speedMultiplier;
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
            collider.enabled = (collider == textures[(int)CurrentMode].sizes[CurrentSize].collider);

        Sprite[] sprites = textures[(int)CurrentMode].sizes[CurrentSize].sprites;
        if (sprites.Length == 1 /*|| !Application.isPlaying*/) {
            spriteRenderer.sprite = sprites[0];
            animating = false;
        }
        else if (!animating) {
            StartAnimation();
        }
        #region new - first document without this, show issue.
        else {
            // animation is already in progress
            foreach (Sprite sprite in sprites)
                if (spriteRenderer.sprite == sprite)
                    return;

            foreach (var mode in textures) {
                foreach (var size in mode.sizes) {
                    for (int i = 0; i < size.sprites.Length; i++) {
                        if (spriteRenderer.sprite == size.sprites[i]) {
                            spriteRenderer.sprite = sprites[i];
                            return;
                        }
                    }   // switch the texture early, because animation will take a while to switch.
                }
            }
        }

        #endregion
    }


    public async void StartAnimation()
    {
        animating = true;
        int i = 0;

        while (animating) {
            Sprite[] sprites = textures[(int)CurrentMode].sizes[CurrentSize].sprites;
            spriteRenderer.sprite = sprites[i];
            await Task.Delay((int)(1000 / animationSpeed));
            i = (i + 1) % sprites.Length;
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (modes.Length != Enum.GetValues(typeof(ModesEnum)).Length || textures.Length != Enum.GetValues(typeof(ModesEnum)).Length || sizes.Length > 5) {
            Array.Resize(ref modes, Enum.GetValues(typeof(ModesEnum)).Length);
            Array.Resize(ref textures, Enum.GetValues(typeof(ModesEnum)).Length);
            Array.Resize(ref sizes, 5);
            Debug.LogError("Incorrect array length!");
        }
        foreach (ModeTextures modeTextures in textures) {
            if (modeTextures.sizes.Length != 5) {
                Debug.LogError("Incorrect array length!");
                Array.Resize(ref modeTextures.sizes, 5);
            }
            foreach (ModeTextures.SizeTextures size in modeTextures.sizes) {
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

        for (int i = 0; i < textures.Length; i++) {
            textures[i]._name = Enum.GetName(typeof(ModesEnum), i);

            for (int j = 0; j < textures[i].sizes.Length; j++) {
                textures[i].sizes[j]._name = $"Size {j}";
            }
        }
        CurrentSize = _currentSize;
        CurrentMode = _currentMode;
    }
#endif

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
public class ModeTextures
{
    [HideInInspector]
    public string _name;

    public bool flippable;
    [NonReorderable]
    public SizeTextures[] sizes = new SizeTextures[5];

    [Serializable]
    public class SizeTextures
    {
        [HideInInspector]
        public string _name;

        public Collider2D collider;
        [Tooltip("Sprite(s) to use for this size. Multiple for idle animation")]
        public Sprite[] sprites = new Sprite[1];
    }
}
