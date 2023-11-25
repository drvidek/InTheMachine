using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPalette), true)]
[CanEditMultipleObjects]
public class ColorPaletteEditor : Editor
{
    private SerializedProperty colors;

    private void OnEnable()
    {
        //initialise fields
        colors = serializedObject.FindProperty("color");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ColorPalette palette = (ColorPalette)target;

        //draw fields
        for (int i = 0; i < palette.color.Length; i++)
        {
            palette.color[i] = EditorGUILayout.ColorField($"{(ColorType)i}", palette.color[i]);
        }

        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}


