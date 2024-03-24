using System.Collections;
using System.Collections.Generic;
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
        //return (bool)Physics2D.OverlapCircle(point, radius, layers);
        return (bool)Physics2D.OverlapArea(TopLeft, BottomRight, layers);
    }
    private void OnDrawGizmos()
    {
        if (showGizmo) {
            //Gizmos.DrawWireSphere(point, radius);
            Gizmos.DrawLineStrip( new Vector3[]{
                TopLeft,
                new Vector2(BottomRight.x, TopLeft.y),
                BottomRight,
                new Vector2(TopLeft.x, BottomRight.y)
            }, true);
        }
    }

}
