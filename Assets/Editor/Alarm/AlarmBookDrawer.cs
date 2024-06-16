using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QKit
{
	[CustomPropertyDrawer(typeof(AlarmBook<>))]
	public class AlarmBookDrawer : PropertyDrawer
	{
		public float lineH = EditorGUIUtility.singleLineHeight;
		public float lineBreak = EditorGUIUtility.singleLineHeight + 4;
		public float lineCount = 1f;

		public float LineCount => lineCount; //+ (showAny ? 2.5f : 0) + (showAny && showAll ? 2.75f : 0);

		public bool setup = false;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!setup)
			{
				//perform initialisation here
				setup = true;
			}

			float defaultLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUI.BeginProperty(position, label, property);

			//Get property relatvies
			var alarmBook = property.FindPropertyRelative("alarmBook");


			//draw background
			Rect bg = position;
			bg.height -= 2f;
			GUI.Box(bg, GUIContent.none);

			//initialise drawing variables
			float x = position.x;
			float y = position.y;
			float w = position.width;
			float fieldWidth = 40f;

			//set your fields here
			Rect nameRect = new Rect(x, y, w / 3, lineH);

			y += lineBreak;

			//draw your fields here
			EditorGUI.LabelField(nameRect, property.displayName);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return lineBreak * LineCount;
		}
	}
}