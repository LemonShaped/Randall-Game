using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CompositeCollider2D))]
public class IndirectCompositeCollider : MonoBehaviour
{
    [SerializeField] CompositeCollider2D composite;

    [SerializeField] ColliderAndOperation[] sourceColliders;

    [SerializeField] bool enableEdit;


    [HideInInspector] [SerializeField] List<Collider2D> generatedColliders;

    [HideInInspector] public List<int> foldComponentIDs = new();

    bool generatedThisFrame;


    [Serializable]
    class ColliderAndOperation
    {
        public Collider2D collider;

        [Tooltip("Composite order is the list index")]
        public Collider2D.CompositeOperation operation;

        public void Deconstruct(out Collider2D first, out Collider2D.CompositeOperation second)
        {
            first = collider;
            second = operation;
        }
    }


    void Awake()
    {
        composite = GetComponent<CompositeCollider2D>();
    }

#if UNITY_EDITOR

    void OnValidate()
    {
        if (Selection.activeGameObject != gameObject)
            return;

        EditorApplication.delayCall += () =>
        {
            if (!enableEdit)
            {
                if (!generatedThisFrame)
                    Generate();

                generatedThisFrame = true;
            }
            else
            {
                foreach (Collider2D collider in generatedColliders)
                    DestroyImmediate(collider);

                generatedColliders.Clear();
            }
        };
    }

    void Update()
    {
        generatedThisFrame = false;
    }

    void Generate()
    {
        if (this == null || Application.isPlaying)
            return;

        foreach (Collider2D col in generatedColliders)
            if (col && col.gameObject == gameObject)
                DestroyImmediate(col);

        generatedColliders.Clear();

        foldComponentIDs.Clear();
        for (int i = 0; i < sourceColliders.Length; i++)
        {
            (Collider2D srcCollider, Collider2D.CompositeOperation operation) = sourceColliders[i];

            if (srcCollider.gameObject == gameObject)
            {
                srcCollider.compositeOperation = Collider2D.CompositeOperation.None;
                srcCollider.enabled = false;
            }

            Collider2D newCol = (Collider2D)gameObject.AddComponent(srcCollider.GetType());

            EditorUtility.CopySerialized(srcCollider, newCol);

            newCol.enabled = true;
            newCol.compositeOperation = operation;
            newCol.compositeOrder = i;
            generatedColliders.Add(newCol);

            // UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(newCol, false);
            foldComponentIDs.Add(newCol.GetInstanceID());
        }

        if (composite.generationType == CompositeCollider2D.GenerationType.Manual)
            composite.GenerateGeometry();
    }


#endif
}
