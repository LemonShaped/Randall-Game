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
