using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QKit
{
    [CustomEditor(typeof(ScoreKeeper), true)]
    [CanEditMultipleObjects]
    public class ScoreKeeperEditor : Editor
    {
        private SerializedProperty _score;
        private SerializedProperty _useMultiplier;
        private SerializedProperty _multiplier;
        private SerializedProperty _multiplierMin;

        private void OnEnable()
        {
            //initialise fields
            _score = serializedObject.FindProperty("_score");
            _useMultiplier = serializedObject.FindProperty("_useMultiplier");
            _multiplier = serializedObject.FindProperty("_multiplier");
            _multiplierMin = serializedObject.FindProperty("_multiplierMin");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            //draw fields
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth / 2;

            _score.floatValue = EditorGUILayout.FloatField("Score:", _score.floatValue);
            EditorGUIUtility.labelWidth = defaultLabelWidth / 3;

            _useMultiplier.boolValue = EditorGUILayout.ToggleLeft("Use Multiplier", _useMultiplier.boolValue);

            EditorGUILayout.EndHorizontal();

            if (_useMultiplier.boolValue)
            {
                EditorGUIUtility.labelWidth = defaultLabelWidth / 2;

                EditorGUILayout.BeginHorizontal();
                _multiplier.floatValue = EditorGUILayout.FloatField("Multiplier:", _multiplier.floatValue);
                _multiplierMin.floatValue = EditorGUILayout.FloatField("Minimum:", _multiplierMin.floatValue);
                //EditorGUILayout.PropertyField(_multiplierMin);
                //EditorGUILayout.PropertyField(_multiplier);
                EditorGUILayout.EndHorizontal();

            }

            if (target)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

    }



}
