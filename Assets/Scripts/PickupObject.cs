using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerController player;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (player != null) { // we are attached to a player

            Vector2 netForce = Vector2.zero;

            Vector2 position = player.rb.position;


            //netForce += (player.rb.position - position) * 100f;


            //position = MyPhysics.CalculateDisplacement(netForce, rb.mass, Time.fixedDeltaTime);

            rb.MovePosition(position);

        }
    }

    protected virtual void OnPickUp(PlayerController byPlayer)
    {
        player = byPlayer;
        transform.parent = byPlayer.inventory;
        rb.gravityScale = 0;
    }

    protected virtual void OnDrop(PlayerController byPlayer)
    {
        player = null;
        transform.parent = null;
        rb.gravityScale = 1;
    }

    public virtual bool CanBePickedUp(PlayerController byPlayer)
    {
        return player == null && (byPlayer.CurrentMode == ModesEnum.Liquid || byPlayer.CurrentMode == ModesEnum.Jelly);
    }

    public virtual bool CanBeDropped(PlayerController byPlayer)
    {
        return true;
    }

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
