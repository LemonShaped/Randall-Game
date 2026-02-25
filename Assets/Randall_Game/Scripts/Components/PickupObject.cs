using UnityEngine;

public class PickupObject : MonoBehaviour
{
    GameManager gameManager;

    public Rigidbody2D rb;
    public PlayerController owner;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public void FixedUpdate()
    {
        if (!owner) return; // Are we attached to a player

        Vector2 position = owner.rb.position;

        //Vector2 netForce = Vector2.zero;
        //netForce += (player.rb.position - position) * 100f;
        //position = MyPhysics.CalculateDisplacement(netForce, rb.mass, Time.fixedDeltaTime);

        rb.MovePosition(position);
    }

    protected virtual void OnPickUp(PlayerController byPlayer)
    {
        owner = byPlayer;
        byPlayer.inventory.Add(this);
        transform.parent = byPlayer.inventoryObj;
        rb.gravityScale = 0;
    }

    protected virtual void OnDrop(PlayerController byPlayer)
    {
        owner = null;
        byPlayer.inventory.Remove(this);
        transform.parent = gameManager.unheldItemParent;
        rb.gravityScale = 1;
    }

    bool CanBePickedUp(PlayerController byPlayer)
    {
        return !owner && byPlayer.CurrentMode is ModesEnum.Liquid or ModesEnum.Jelly;
    }

    static bool CanBeDropped(PlayerController byPlayer)
    {
        return true;
    }

    /// <returns>Success picking up</returns>
    public bool PickUp(PlayerController byPlayer)
    {
        if (!CanBePickedUp(byPlayer)) {
            return false;
        }
        OnPickUp(byPlayer);
        return true;
    }

    public bool Drop(PlayerController byPlayer)
    {
        if (!CanBeDropped(byPlayer)) {
            return false;
        }
        OnDrop(byPlayer);
        return true;
    }
}
