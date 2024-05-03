using System;
using System.Collections.Generic;
using UnityEngine;


using UnityEditor;

[CustomEditor(typeof(EnemyList), true)]
[CanEditMultipleObjects]
public class EnemyListDrawer : Editor
{
    private SerializedProperty enemyPrefabs;

    GameObject[] enemies = new GameObject[EnemyList.Length];

    private void OnEnable()
    {
        EnemyList list = (EnemyList)target;
        enemies = list.enemyPrefabs;
        enemyPrefabs = serializedObject.FindProperty("enemyPrefabs");
        if (enemies.Length < EnemyList.Length)
        {
            GameObject[] newColors = new GameObject[EnemyList.Length];
            enemies.CopyTo(newColors, 0);
            enemies = newColors;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //draw fields
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = EditorGUILayout.ObjectField($"{(EnemyList.Type)i}", enemies[i],typeof(GameObject),false) as GameObject;
            if (enemyPrefabs.arraySize > i)
                enemyPrefabs.DeleteArrayElementAtIndex(i);
            enemyPrefabs.InsertArrayElementAtIndex(i);
            enemyPrefabs.GetArrayElementAtIndex(i).objectReferenceValue = enemies[i];

        }
        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}


