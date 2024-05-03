using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPalette), true)]
[CanEditMultipleObjects]
public class ColorPaletteEditor : Editor
{

    SerializedProperty colorsSerialised;
    Color[] colors = new Color[ColorPalette.Length];

    private void OnEnable()
    {
        ColorPalette palette = (ColorPalette)target;
        colors = palette.color;
        colorsSerialised = serializedObject.FindProperty("color");
        if (colors.Length < ColorPalette.Length)
        {
            Color[] newColors = new Color[ColorPalette.Length];
            colors.CopyTo(newColors, 0);
            colors = newColors;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //draw fields
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = EditorGUILayout.ColorField($"{(ColorType)i}", colors[i]);
            if (colorsSerialised.arraySize > i)
                colorsSerialised.DeleteArrayElementAtIndex(i);
            colorsSerialised.InsertArrayElementAtIndex(i);
            colorsSerialised.GetArrayElementAtIndex(i).colorValue = colors[i];

        }
        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}


