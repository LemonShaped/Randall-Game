using UnityEngine;

public class Jelly : PickupObject
{

    public SpriteRenderer sprite;


    public override bool CanPickUp(PlayerController player)
    {
        return player.CurrentMode == ModesEnum.Liquid;
    }
    public override void OnPickup(PlayerController player)
    {
        base.OnPickup(player);
        sprite.enabled = false;
        player.CurrentMode = ModesEnum.Jelly;
    }
    public override void OnDrop(PlayerController player)
    {
        sprite.enabled = true;
        if (player.CurrentMode == ModesEnum.Jelly)
            player.CurrentMode = ModesEnum.Liquid;
        base.OnDrop(player);
    }
}
