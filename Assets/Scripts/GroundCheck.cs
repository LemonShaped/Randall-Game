using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Vector2 pointA;
    public Vector2 pointB;
    public bool mirror;
    public bool showGizmo;

    private void OnValidate() {
        if (mirror)
            pointB.x = -pointA.x;
    }

    public Vector2 TopLeft {
        get => (Vector2)transform.position + pointA;
        set => pointA = value - (Vector2)transform.position;
    }
    public Vector2 BottomRight {
        get => (Vector2)transform.position + pointB;
        set => pointB = value - (Vector2)transform.position;
    }

    public bool CheckGround(LayerMask layers)
    {
        return (bool)Physics2D.OverlapArea(TopLeft, BottomRight, layers);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmo) {
            Handles.DrawSolidRectangleWithOutline(new Vector3[]{
                TopLeft,
                new Vector2(BottomRight.x, TopLeft.y),
                BottomRight,
                new Vector2(TopLeft.x, BottomRight.y)
            },
            CheckGround(GetComponent<LiquidCharacter>().groundLayers) ? new Color(0f, 1f, 0f, 0.3f) : Color.clear,
            Color.white);
        }
    }

}
