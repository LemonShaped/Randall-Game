using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class GenerateMesh : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Collider2D collider2d;

    public string meshName;
    public Mesh meshAsset;

    void Start() {
        if (Application.isPlaying) {
            meshFilter.mesh = meshAsset;
        }
    }


#if UNITY_EDITOR

    public bool saveNow;

    public bool delete;

    void Update() {

        if (!Application.isPlaying && (Selection.activeGameObject == gameObject
                || (collider2d && Selection.activeGameObject == collider2d.gameObject)
                || (meshFilter && Selection.activeGameObject == meshFilter.gameObject))) {

            if (!meshAsset || (meshFilter && meshFilter.sharedMesh != meshAsset))
                saveNow = true;

            if (meshName is null or "")
                meshName = gameObject.name;

            // Rename generated mesh asset to match
            if (meshAsset && meshAsset.name != meshName && AssetDatabase.GetAssetPath(meshAsset) == GeneratedMeshes.AssetPath) {
                meshAsset.name = meshName;
                EditorUtility.SetDirty(meshAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Mesh colliderMesh = null;


            // Generate mesh from collider
            if (collider2d) {

                colliderMesh = collider2d.CreateMesh(true, true);

                colliderMesh.vertices = Array.ConvertAll(colliderMesh.vertices,
                    (Vector3 point) => point - transform.position);

                colliderMesh.RecalculateNormals();
                colliderMesh.RecalculateTangents();
                colliderMesh.RecalculateBounds();

                colliderMesh.uv = Array.ConvertAll(colliderMesh.vertices,
                    (Vector3 vertex) => new Vector2(
                        Mathf.InverseLerp(colliderMesh.bounds.min.x, colliderMesh.bounds.max.x, vertex.x),
                        Mathf.InverseLerp(colliderMesh.bounds.min.y, colliderMesh.bounds.max.y, vertex.y))
                );

            }

            if (delete)
            {
                var mesh = AssetDatabase.LoadAllAssetsAtPath(GeneratedMeshes.AssetPath)
                    .FirstOrDefault(obj => obj is Mesh && obj.name == meshName);
                    // .Select((Object obj) => obj.name);
                // foreach (var s in mesh)
                //     Debug.Log(s);
                if (mesh)
                    AssetDatabase.RemoveObjectFromAsset(mesh);

                saveNow = false;
                delete = false;
                return;
            }

            // Save mesh to file
            if (saveNow) {

                Mesh mesh;
                if (collider2d == null)
                    mesh = Instantiate(meshFilter.sharedMesh);
                else
                    mesh = Instantiate(colliderMesh);

                mesh.name = meshName;

                if (!AssetDatabase.AssetPathExists(GeneratedMeshes.AssetPath)) {
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GeneratedMeshes>(), GeneratedMeshes.AssetPath);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                GeneratedMeshes generatedMeshes = AssetDatabase.LoadAssetAtPath<GeneratedMeshes>(GeneratedMeshes.AssetPath);


                Undo.RecordObject(generatedMeshes, "Save mesh to GeneratedMeshes");


                meshAsset = (Mesh)AssetDatabase.LoadAllAssetsAtPath(GeneratedMeshes.AssetPath).FirstOrDefault(obj => obj is Mesh && obj.name == meshName);
                if (!meshAsset) {
                    AssetDatabase.AddObjectToAsset(mesh, generatedMeshes);
                    EditorUtility.SetDirty(generatedMeshes);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    meshAsset = (Mesh)AssetDatabase.LoadAllAssetsAtPath(GeneratedMeshes.AssetPath).First(obj => obj is Mesh && obj.name == meshName);
                }

                meshAsset.triangles = mesh.triangles;
                meshAsset.vertices = mesh.vertices;
                meshAsset.uv = mesh.uv;
                meshAsset.RecalculateNormals();
                meshAsset.RecalculateTangents();
                meshAsset.RecalculateBounds();


                EditorUtility.SetDirty(meshAsset);
                EditorUtility.SetDirty(generatedMeshes);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Saved {meshAsset}");

            }
            saveNow = false;

            if (meshFilter && meshAsset)
                meshFilter.sharedMesh = meshAsset;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        if (collider2d is PolygonCollider2D polygon)
            Gizmos.DrawLineStrip(polygon.points.Select(v2 => (Vector3)v2 + transform.position).ToArray(), true);
        //else if (meshFilter)
        //    Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position);
    }


#endif
}
