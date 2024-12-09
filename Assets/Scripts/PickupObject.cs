using UnityEngine;

public class PickupObject : MonoBehaviour
{

    public virtual void OnPickup(PlayerController player)
    {
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
    }

    public virtual bool CanPickUp(PlayerController player)
    {
        return true;
    }

    public virtual void OnDrop(PlayerController player)
    {
        transform.parent = null;
    }

    public virtual bool CanDrop(PlayerController player)
    {
        return true;
    }
}
