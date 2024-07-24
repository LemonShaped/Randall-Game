using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GenerateMesh))]
public class GenerateMesh_Inspector : Editor
{
    public override VisualElement CreateInspectorGUI() {
        VisualElement inspector = new VisualElement();

        InspectorElement.FillDefaultInspector(inspector, serializedObject, this);

        if (serializedObject.FindProperty(nameof(GenerateMesh.meshFilter)).objectReferenceValue == null
        && serializedObject.FindProperty(nameof(GenerateMesh.collider2d)).objectReferenceValue == null) {
            inspector.Add(new HelpBox("Will not do anything without at least either a meshFilter or a collider2d.", HelpBoxMessageType.Warning));
        }


        return inspector;
    }
}
