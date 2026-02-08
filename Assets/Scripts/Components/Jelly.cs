using UnityEngine;

public class Jelly : PickupObject
{

    public SpriteRenderer sprite;


    protected override void OnPickUp(PlayerController player)
    {
        base.OnPickUp(player);
        sprite.enabled = false;
        player.CurrentMode = ModesEnum.Jelly;
    }
    protected override void OnDrop(PlayerController player)
    {
        base.OnDrop(player);
        sprite.enabled = true;
        if (player.CurrentMode == ModesEnum.Jelly)
            player.CurrentMode = ModesEnum.Liquid;
    }
}
