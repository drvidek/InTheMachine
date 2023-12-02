using UnityEditor;
using UnityEngine;

namespace QKit
{
    [CustomPropertyDrawer(typeof(Alarm))]
    public class AlarmDrawer : PropertyDrawer
    {
        private float lineH = EditorGUIUtility.singleLineHeight;
        private float lineBreak = EditorGUIUtility.singleLineHeight + 4;
        private float lineCount = 4f;
        public float LineCount => lineCount - (showAll ? 0 : 2);

        public bool showAll;

        public bool setup = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!setup)
            {
                showAll = AlarmEditorWindow.showAllDefault;
                setup = true;
            }

            EditorGUI.BeginProperty(position, label, property);

            var name = property.FindPropertyRelative("_name");
            var id = property.FindPropertyRelative("_id");
            var timeRemaining = property.FindPropertyRelative("_timeRemaining");
            var timeMax = property.FindPropertyRelative("_timeMax");
            var timeScale = property.FindPropertyRelative("_timeScale");
            var looping = property.FindPropertyRelative("_looping");
            var paused = property.FindPropertyRelative("_paused");
            var stopped = property.FindPropertyRelative("_stopped");
            var timeType = property.FindPropertyRelative("_type");

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            float percent = Mathf.Clamp01((timeRemaining.floatValue) / (timeMax.floatValue));

            Rect bg = position;
            bg.height -= 2f;

            GUI.Box(bg, GUIContent.none);

            float x = position.x;
            float y = position.y;

            float currentY = y;

            float w = position.width;
            float valwidth = 40f;
            float buttonWidth = (w - valwidth * 2) / 3f;

            Rect classLabelRect = new Rect(x, currentY, w, lineH);
            //Rect nameRect = new Rect(x + valwidth, currentY, w - valwidth, lineH);
            Rect playPauseRect = new Rect(x + valwidth * 2, currentY, buttonWidth, lineH);
            Rect stopRect = new Rect(x + valwidth * 2 + buttonWidth, currentY, buttonWidth, lineH);
            Rect resetRect = new Rect(x + valwidth * 2 + buttonWidth * 2, currentY, buttonWidth, lineH);

            //currentY += lineBreak;


            currentY += lineBreak;

            float valWidthAdjusted = Mathf.Min(valwidth * 2, w / 4);
            float totalWidth = valWidthAdjusted * 4.5f;

            Rect timeRemainingRect = new Rect(x + valwidth * 2, currentY, valWidthAdjusted, lineH);
            Rect ofLabelRect = new Rect(x + valwidth * 2 + valWidthAdjusted, currentY, valWidthAdjusted / 2, lineH);
            Rect timeMaxRect = new Rect(x + valwidth * 2 + valWidthAdjusted * 1.5f, currentY, valWidthAdjusted, lineH);
            Rect secRemainingLabelRect = new Rect(x + valwidth * 2 + valWidthAdjusted * 2.5f, currentY, Mathf.Max(valWidthAdjusted * 2, 32), lineH);

            currentY += lineBreak;

            valWidthAdjusted = Mathf.Min(valwidth * 2.5f, w / 2);

            Rect loopingRect = new Rect(x + 2, currentY, valWidthAdjusted, lineH);
            Rect timeScaleRect = new Rect(w - 2 - valWidthAdjusted, currentY, valWidthAdjusted, lineH);

            currentY += lineBreak;

            //valWidthAdjusted = Mathf.Min(valwidth * 2, w / 2);

            Rect timeTypeRect = new Rect(x + 2, currentY, w, lineH);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.LabelField(classLabelRect, property.displayName);
            //name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);

            EditorGUI.BeginDisabledGroup(AlarmEditorWindow.lockActionButtons);
            if (paused.boolValue == true || stopped.boolValue == true)
            {
                if (GUI.Button(playPauseRect, "Play"))
                {
                    paused.boolValue = false;
                    stopped.boolValue = false;
                }
            }
            else
            if (GUI.Button(playPauseRect, "Pause"))
            {
                paused.boolValue = true;
            }
            if (GUI.Button(stopRect, "Stop"))
            {
                stopped.boolValue = true;
                timeRemaining.floatValue = -1;
            }
            if (GUI.Button(resetRect, "Reset"))
            {
                timeRemaining.floatValue = timeMax.floatValue;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(AlarmEditorWindow.disableAlarmChanges);
            timeRemaining.floatValue = EditorGUI.FloatField(timeRemainingRect, Mathf.Max(ClipToDecimalPlace(timeRemaining.floatValue, Alarm.alarmPrecision), 0f));
            EditorGUI.LabelField(ofLabelRect, "of", style);
            timeMax.floatValue = EditorGUI.FloatField(timeMaxRect, ClipToDecimalPlace(timeMax.floatValue, Alarm.alarmPrecision));

            style.alignment = TextAnchor.MiddleLeft;
            EditorGUI.LabelField(secRemainingLabelRect, "sec remaining", style);

            showAll = EditorGUI.Foldout(classLabelRect, showAll, "", false);
            if (showAll)
            {
                EditorGUIUtility.labelWidth = 55f;

                looping.boolValue = EditorGUI.Toggle(loopingRect, "Looping: ", looping.boolValue);

                EditorGUIUtility.labelWidth = 75f;

                timeScale.floatValue = EditorGUI.FloatField(timeScaleRect, "Time scale: ", timeScale.floatValue);

                EditorGUI.PropertyField(timeTypeRect, timeType);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        private float ClipToDecimalPlace(float t, float decimals)
        {
            float precision = Mathf.Max(1, Mathf.Pow(10, decimals));
            return Mathf.Ceil(t * precision) / precision;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return lineBreak * LineCount;
        }
    }

}