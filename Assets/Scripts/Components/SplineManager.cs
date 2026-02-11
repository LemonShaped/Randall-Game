using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;

public class SplineManager : MonoBehaviour
{
    public SpriteShapeController[] spriteShapeControllers;

    public SplineContainer splineContainer;
    public int splineIndex;

    public UnityEngine.Splines.Spline spline => splineContainer.Splines[splineIndex];

    [SerializeField] bool recreate;

    public new PolygonCollider2D collider;

    [SerializeField] new bool print;

    void Recreate()
    {
        foreach (var c in spriteShapeControllers) {
            UnityEngine.U2D.Spline spriteSpline = c.spline;

            spriteSpline.Clear();
            spriteSpline.isOpenEnded = false;

            int ssPointCount = spriteSpline.GetPointCount();
            for (int index = 0; index < Mathf.Max(ssPointCount, spline.Count); index++) {

                if (index >= ssPointCount)
                    spriteSpline.InsertPointAt(index, spline[index].Position);

                else if (index >= spline.Count) {
                    spriteSpline.RemovePointAt(index);
                    continue;
                }

                spriteSpline.SetCorner(index, true);
                spriteSpline.SetHeight(index, 1f);
                spriteSpline.SetPosition(index, spline[index].Position);
                spriteSpline.SetSpriteIndex(index, 0);

                if (spline.GetTangentMode(index) == TangentMode.Linear) {
                    spriteSpline.SetTangentMode(index, ShapeTangentMode.Linear);
                    spriteSpline.SetLeftTangent(index, Vector3.zero);
                    spriteSpline.SetRightTangent(index, Vector3.zero);
                }
                else {
                    spriteSpline.SetTangentMode(index, ShapeTangentMode.Broken);
                    spriteSpline.SetLeftTangent(index, math.rotate(spline[index].Rotation, spline[index].TangentIn));
                    spriteSpline.SetRightTangent(index, math.rotate(spline[index].Rotation, spline[index].TangentOut));
                }

            }
        }
        collider = spriteShapeControllers[0].polygonCollider;
        spriteShapeControllers[0].BakeCollider();
    }

    void Print()
    {
        var log = "";

        var count = spriteShapeControllers[0].spline.GetPointCount();
        log += "Point count " + count;

        for (int index = 0; index < count; index++) {
            log += "\n\nindex: " + index;
            log += "\nPosition " + spriteShapeControllers[0].spline.GetPosition(index);
            log += "\nLeftTangent" + spriteShapeControllers[0].spline.GetLeftTangent(index);
            log += "\nRightTangent" + spriteShapeControllers[0].spline.GetRightTangent(index);
        }

        Debug.Log(log);
    }

    private void OnValidate()
    {
        if (recreate) {
            recreate = false;
            Recreate();
        }
        if (print) {
            print = false;
            Print();
        }
    }
}
