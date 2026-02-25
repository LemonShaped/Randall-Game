using UnityEngine;

public class Jelly : PickupObject
{

    public SpriteRenderer sprite;


    protected override void OnPickUp(PlayerController byPlayer)
    {
        base.OnPickUp(byPlayer);
        sprite.enabled = false;
        byPlayer.CurrentMode = ModesEnum.Jelly;
    }
    protected override void OnDrop(PlayerController byPlayer)
    {
        base.OnDrop(byPlayer);
        sprite.enabled = true;
        if (byPlayer.CurrentMode == ModesEnum.Jelly)
            byPlayer.CurrentMode = ModesEnum.Liquid;
    }
}
