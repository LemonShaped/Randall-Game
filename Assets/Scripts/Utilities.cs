using System.Diagnostics.CodeAnalysis;
using UnityEngine;


[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class PureColour
{
    /// <summary> Completely transparent. RGBA is (0, 0, 0, 0). </summary>
    public static readonly Color Clear = new(0f, 0f, 0f, 0f);

    /// <summary> White. RGB is (1, 1, 1). </summary>
    public static readonly Color White = new(1f, 1f, 1f);

    /// <summary> Black. RGB is (0, 0, 0). </summary>
    public static readonly Color Black = new(0f, 0f, 0f);

    /// <summary> Gray. RGB is (0.5, 0.5, 0.5). </summary>
    public static readonly Color Gray = new(0.5f, 0.5f, 0.5f);

    /// <summary> Grey. RGB is (0.5, 0.5, 0.5). </summary>
    public static readonly Color Grey = new(0.5f, 0.5f, 0.5f);

    /// <summary> Red. RGB is (1, 0, 0). </summary>
    public static readonly Color Red = new(1f, 0f, 0f);

    /// <summary> Orange. (Yellow-Red) RGB is (1, 0.5, 0). </summary>
    public static readonly Color Orange = new(1f, 0.5f, 0f);

    /// <summary> Yellow. RGB is (1, 1, 0) </summary>
    public static readonly Color Yellow = new(1f, 1f, 0f);

    /// <summary> Chartreuse. (Yellow-Green) RGB is (0.5, 1, 0). </summary>
    /// <remarks>Close to <see cref="Color.chartreuse"/></remarks>
    public static readonly Color Chartreuse = new(0.5f, 1f, 0f);

    /// <summary> Green. RGB is (0, 1, 0). </summary>
    public static readonly Color Green = new(0f, 1f, 0f);

    /// <summary> Spring Green. (Cyan-Green) RGB is (0, 1, 0.5). </summary>
    public static readonly Color SpringGreen = new(0f, 1f, 0.5f);

    /// <summary> Cyan. RGB is (0, 1, 1). </summary>
    public static readonly Color Cyan = new(0f, 1f, 1f);

    /// <summary> Azure. (Cyan-Blue) RGB is (0, 0.5, 1). </summary>
    /// <remarks>Not like <see cref="Color.azure"/>. Closest to <see cref="Color.dodgerBlue"/></remarks>
    public static readonly Color Azure = new(0f, 0.5f, 1f);

    /// <summary> Blue. RGB is (0, 0, 1). </summary>
    public static readonly Color Blue = new(0f, 0f, 1f);

    /// <summary> Violet. (Magenta-Blue) RGB is (0.5, 0, 1). </summary>
    public static readonly Color Violet = new(0.5f, 0f, 1f);

    /// <summary> Magenta. RGB is (1, 0, 1). </summary>
    public static readonly Color Magenta = new(1f, 0f, 1f);

    /// <summary> Rose. (Magenta-Red) RGB is (1, 0, 0.5). </summary>
    /// <remarks>Deep pink colour. Close to <see cref="Color.deepPink"/></remarks>
    public static readonly Color Rose = new(1f, 0f, 0.5f);

}


[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public static class ExtensionMethods
{
    public static Color WithAlpha(this Color colour, float a)
        => new Color(colour.r, colour.g, colour.b, a);

    /// <summary> Mix (lerp) with the given colour </summary>
    /// <param name="ratio"> percent of the added colour (0-1)</param>
    public static Color MixWith(this Color thisColour, Color colour, float ratio)
        => Color.Lerp(thisColour, colour, ratio);
    public static Color MixWith(this Color thisColour, Color colour)
        => thisColour.MixWith(colour, 0.5f);


    /// <summary> Extension method to check if a layer is in the layer mask </summary>
    public static bool Contains(this LayerMask layerMask, int layerIndex)
        => (layerMask & (1 << layerIndex)) != 0;
    /// <summary> Extension method to check if a layer is in the layer mask </summary>
    public static bool Contains(this LayerMask layerMask, string layerName)
        => layerMask.Contains(LayerMask.NameToLayer(layerName));

    public static LayerMask Union(this LayerMask mask1, LayerMask mask2)
        => mask1 | mask2;
    public static LayerMask Intersection(this LayerMask mask1, LayerMask mask2)
        => mask1 & mask2;
    public static LayerMask Add(this LayerMask layerMask, int layerIndex)
        => layerMask | (1 << layerIndex);
    public static LayerMask Add(this LayerMask layerMask, string layer)
        => layerMask.Add(LayerMask.NameToLayer(layer));
    public static LayerMask Remove(this LayerMask layerMask, int layerIndex)
        => layerMask & ~(1 << layerIndex);
    public static LayerMask Remove(this LayerMask layerMask, string layer)
        => layerMask.Remove(LayerMask.NameToLayer(layer));

}

public static class Util
{
    public static class Vector2
    {
        public static readonly UnityEngine.Vector2 NaN = new UnityEngine.Vector2(float.NaN, float.NaN);
    }

    public static LayerMask LayerMaskFromLayer(int layerIndex)
        => (LayerMask)(1 << layerIndex);

    public static LayerMask LayerMaskFromLayer(string layerName)
        => (LayerMask)(1 << LayerMask.NameToLayer(layerName));

}
public static class MyPhysics
{
    public static Vector2 CalculateDisplacement(Vector2 force, float mass, float time)
    {
        // F=ma
        Vector2 acceleration = force / mass;
        // v=a*t
        Vector2 velocity = acceleration * time;
        // s=v*t
        Vector2 displacement = velocity * time;

        return displacement;
    }
}
