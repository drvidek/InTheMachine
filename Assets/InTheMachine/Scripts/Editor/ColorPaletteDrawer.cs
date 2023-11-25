using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ColorPalette), true)]
[CanEditMultipleObjects]
public class ColorPaletteEditor : Editor
{
    Color[] colors = new Color[(int)ColorType._max];

    SerializedProperty colorsSerialised;

    private void OnEnable()
    {
        ColorPalette palette = (ColorPalette)target;
        colors = palette.color;
        colorsSerialised = serializedObject.FindProperty("color");
        if (colors.Length < (int)ColorType._max)
        {
            Color[] newColors = new Color[(int)ColorType._max];
            colors.CopyTo(newColors, 0);
            colors = newColors;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ColorPalette palette = (ColorPalette)target;

        //draw fields
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = EditorGUILayout.ColorField($"{(ColorType)i}", colors[i]);
        }

        if (GUILayout.Button("Apply colors"))
        {
            palette.color = colors;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}


