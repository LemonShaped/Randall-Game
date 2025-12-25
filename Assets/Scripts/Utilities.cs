using UnityEngine;


public static class Colours
{
    /// <summary> Solid red. RGBA is (1, 0, 0, 1). </summary>
    public static Color Red => Color.red;

    /// <summary> Solid green. RGBA is (0, 1, 0, 1). </summary>
    public static Color Green => Color.green;

    /// <summary> Solid blue. RGBA is (0, 0, 1, 1). </summary>
    public static Color Blue => Color.blue;

    /// <summary> Solid white. RGBA is (1, 1, 1, 1). </summary>
    public static Color White => Color.white;

    /// <summary> Solid black. RGBA is (0, 0, 0, 1). </summary>
    public static Color Black => Color.black;

    /// <summary> Cyan. RGBA is (0, 1, 1, 1). </summary>
    public static Color Cyan => Color.cyan;

    /// <summary> Magenta. RGBA is (1, 0, 1, 1). </summary>
    public static Color Magenta => Color.magenta;

    /// <summary> Yellow. RGBA is (1, 1, 0, 1) </summary>
    public static Color Yellow => new Color(1, 1, 0, 1);

    /// <summary> A nice yellow, but the RGBA is (1, 0.92, 0.016, 1) </summary>
    public static Color Yellow2 => Color.yellow;

    /// <summary> Gray. RGBA is (0.5, 0.5, 0.5, 1). </summary>
    public static Color Gray => Color.gray;

    /// <summary> Grey. RGBA is (0.5, 0.5, 0.5, 1). </summary>
    public static Color Grey => Color.grey;

    /// <summary> Completely transparent. RGBA is (0, 0, 0, 0). </summary>
    public static Color Clear => Color.clear;

    public static Color Orange => Colours.Red.MixWith(Colours.Yellow);

    public static Color Purple => Colours.Red.MixWith(Color.blue);
}


public static class ExtensionMethods
{
    public static Color WithAlpha(this Color colour, float a)
        => new Color(colour.r, colour.g, colour.b, a);

    /// <summary> Mix (lerp) with the given colour </summary>
    /// <param name="ratio"> ratio: percent of the added colour </param>
    public static Color MixWith(this Color thisColour, Color colour, float ratio)
        => Color.Lerp(thisColour, colour, ratio);
    public static Color MixWith(this Color thisColour, Color colour)
        => thisColour.MixWith(colour, 0.5f);


    /// <summary> Extension method to check if a layer is in the layermask </summary>
    public static bool Contains(this LayerMask layerMask, int layerIndex)
        => (layerMask & (1 << layerIndex)) != 0;
    /// <summary> Extension method to check if a layer is in the layermask </summary>
    public static bool Contains(this LayerMask layerMask, string layerName)
        => layerMask.Contains(LayerMask.NameToLayer(layerName));

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

/// <summary>Constants</summary>
public readonly struct Const
{
    public static class Vector2
    {
        public static readonly UnityEngine.Vector2 NaN = new UnityEngine.Vector2(float.NaN, float.NaN);
    }
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


#if false
public class Test : MonoBehaviour
{

    /// <summary>
    /// Must create a non-generic version before usage, eg:
    /// [Serializable] class MeshDictionary : SerializableDict<string, Mesh> { }
    /// [SerializeField] MeshDictionary test = new();
    /// </summary>
    [Serializable]
    private class SerializableDict<TKey, TValue> : Dictionary<TKey, TValue>
    {

        [SerializeField] private Item_[] _items_;

        public SerializableDict() {
            EditorApplication.update += Update;
            EditorApplication.update += OnValidate;
        }

        [Serializable]
        private struct Item_
        {
            public TKey key;
            public TValue value;
            public Item_(KeyValuePair<TKey, TValue> keyValuePair)
                => (key, value) = keyValuePair;
        }

        private void Update() {
            if (!Application.isPlaying) {
                if (!_writeMode)
                    _items_ = this.Select((kvp) => new Item_(kvp)).ToArray();
            }
            else if (_writeMode)
                _writeMode = false;
        }


        // To be editable:

        [Tooltip("Dangerous (completely overwrites every time).\nDisable as soon as done.")]
        [SerializeField]
        private bool _writeMode = false;

        private void OnValidate() {
            if (_writeMode) {
                this.Clear();
                foreach (Item_ item_ in _items_) {
                    this[item_.key] = item_.value;
                }
            }
        }
    }
}
#endif


#if false
private class Chunk
{
    public static int sizeX;
    public static int sizeZ;
    public static int minY; // <= 0
    public static int maxY; // >= 0
    public static int sizeY = maxY - minY + 1;
    // -8 -7 -6 -5 -4 -3 -2 -1 0  1  2  3  4  5  6  7  8

    private Block[,,] positiveY = new Block[sizeX, maxY + 1, sizeZ];
    private Block[,,] negativeY = new Block[sizeX, -minY, sizeZ];

    public Block this[int x, int y, int z] {
        get {
            if (y < 0)
                return negativeY[x, -y - 1, z];
            else
                return positiveY[x, y, z];
        }
        set {
            if (y < 0)
                negativeY[x, -y - 1, z] = value;
            else
                positiveY[x, y, z] = value;
        }
    }
    public class Block { }
}

#endif
