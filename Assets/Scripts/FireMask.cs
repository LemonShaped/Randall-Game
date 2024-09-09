#if UNITY_EDITOR

using System;
using System.Collections;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


[RequireComponent(typeof(SpriteMask))]
public class FireMask : MonoBehaviour
{

    public SpriteMask mask;
    public Vector2 size;
    public Vector2 offset;

    private void OnValidate()
    {
        SpriteMask mask = gameObject.GetComponent<SpriteMask>();
        Debug.Log(AssetDatabase.GetAssetPath(this));
        mask.sprite = Sprite.Create(new Texture2D(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y)), new Rect(Vector2.zero, size), new Vector2(0.5f, 0) + offset, 1f);
        mask.sprite.name = "Generated sprite";
    }

}


#endif
