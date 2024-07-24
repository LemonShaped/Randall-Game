using UnityEngine;

public static class HelperMethods
{
    public static Color WithAlpha(this Color color, float a) {
        return new Color(color.r, color.g, color.b, a);
    }
    public static Color MixWith(this Color thisColor, Color color) {
        return thisColor.MixWith(color, 0.5f);
    }
    /// <summary>Lerps colours. ratio = percent being the new color.</summary>
    public static Color MixWith(this Color thisColor, Color color, float ratio) {
        return Color.Lerp(thisColor, color, ratio);
    }

    /// <summary> Set to the color yellow (RGB: 1, 1, 0) </summary>
    public static Color Yellow(this Color _)
        => new Color(1, 1, 0, 1);
    public static Color Orange(this Color _)
        => Color.red.MixWith(new Color().Yellow());
    public static Color Purple(this Color _)
        => Color.red.MixWith(Color.blue);



    /// <summary> Extension method to check if a layer is in a layermask </summary>
    public static bool ContainsLayer(this LayerMask layerMask, string layerName)
        => layerMask.ContainsLayer(LayerMask.NameToLayer(layerName));
    /// <summary> Extension method to check if a layer is in a layermask </summary>
    public static bool ContainsLayer(this LayerMask layerMask, int layerIndex)
        => (layerMask & (1 << layerIndex)) != 0;


    public static LayerMask Union(this LayerMask mask1, LayerMask mask2)
        => mask1 | mask2;
    public static LayerMask Intersection(this LayerMask mask1, LayerMask mask2)
        => mask1 & mask2;
    public static LayerMask Including(this LayerMask layerMask, int layerIndex)
        => layerMask | (1 << layerIndex);
    public static LayerMask Including(this LayerMask layerMask, string layer)
        => layerMask.Including(LayerMask.NameToLayer(layer));
    public static LayerMask Excluding(this LayerMask layerMask, int layerIndex)
        => layerMask & ~(1 << layerIndex);
    public static LayerMask Excluding(this LayerMask layerMask, string layer)
        => layerMask.Excluding(LayerMask.NameToLayer(layer));



}
