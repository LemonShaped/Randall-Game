using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
[CustomEditor(typeof(IndirectCompositeCollider))]
public class IndirectCompositeCollider_Editor : Editor
{
    void OnEnable()
    {
        IndirectCompositeCollider indirectCompositeCollider = (IndirectCompositeCollider)target;

        Selection.activeTransform = indirectCompositeCollider.transform;

        EditorApplication.delayCall += () =>
        {
            if (this == null || indirectCompositeCollider == null) return;

            // Get the internal InspectorWindow type
            Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            if (inspectorType == null)
            {
                Debug.LogError("InspectorWindow type not found");
                return;
            }

            // Find an existing inspector window
            var inspector = (EditorWindow)Resources.FindObjectsOfTypeAll(inspectorType).FirstOrDefault();
            if (inspector == null)
            {
                Debug.LogError("No InspectorWindow found");
                return;
            }

            FieldInfo trackerField =
                inspectorType.GetField("m_Tracker", BindingFlags.Instance | BindingFlags.NonPublic);
            if (trackerField == null)
            {
                Debug.LogError("m_Tracker field not found");
                return;
            }

            var tracker = (ActiveEditorTracker)trackerField.GetValue(inspector);
            if (tracker == null) return;

            Editor[] editors = tracker.activeEditors;

            for (int i = 0; i < editors.Length; i++)
            {
                Editor ed = editors[i];
                if (ed == null || ed.target == null)
                {
                    Debug.LogWarning("Editor not found");
                    continue;
                }

                if (indirectCompositeCollider.foldComponentIDs.Contains(ed.target.GetInstanceID()))
                    tracker.SetVisible(i, 0);
                // else if (ed.target is Collider2D)
                //     tracker.SetVisible(i, 1);
            }

            inspector.Repaint();
        };
        ;
    }
}
