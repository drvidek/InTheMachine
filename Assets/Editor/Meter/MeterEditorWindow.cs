using System.IO;
using System;
using UnityEngine;
using UnityEditor;

namespace QKit
{
    public class MeterEditorWindow : EditorWindow
    {
        [MenuItem("QKit/Meters")]
        public static void ShowWindow()
        {
            thisWindow = GetWindow(typeof(MeterEditorWindow), false, "Meters");
            windowRect = thisWindow.position;
            if (File.Exists(path))
                LoadOptions();
        }

        private static Rect windowRect;
        private static EditorWindow thisWindow;

        private Vector2 windowScrollPos;

        public static bool showAnyDefault = true, showAllDefault;
        public static Color defaultMeterColor = Color.cyan, defaultMeterBgColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        public static Gradient defaultMeterGradient = new Gradient();

        private static string path = Path.Combine(Application.streamingAssetsPath, "QKitResources/Editor/Meter.txt");

        private void OnGUI()
        {
            if (thisWindow == null)
                thisWindow = GetWindow(typeof(MeterEditorWindow));
            if (windowRect.size != thisWindow.position.size)
            {
                windowRect = thisWindow.position;
            }

            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

            //Insert GUI code here//
            showAnyDefault = EditorGUILayout.ToggleLeft("Show the expanded Meter in the inspector by default", showAnyDefault);
            showAllDefault = EditorGUILayout.ToggleLeft("Show Meter color options in the inspector by default", showAllDefault);
            EditorGUI.BeginChangeCheck();
            defaultMeterColor = EditorGUILayout.ColorField("Default Meter Color", defaultMeterColor);
            defaultMeterBgColor = EditorGUILayout.ColorField("Default Meter Background Color", defaultMeterBgColor);
            defaultMeterGradient = EditorGUILayout.GradientField("Default Meter Gradient", defaultMeterGradient);

            if (EditorGUI.EndChangeCheck())
            {
                Meter.defaultMeterColor = defaultMeterColor;
                Meter.defaultMeterBgColor = defaultMeterBgColor;
                Meter.defaultGradient = defaultMeterGradient;
            }

            EditorGUILayout.EndScrollView();
        }


        private void OnDestroy()
        {
            SaveOptions();
        }

        private void SaveOptions()
        {
            StreamWriter writer = new(path, false);
            writer.WriteLine(showAnyDefault);
            writer.WriteLine(showAllDefault);
            writer.WriteLine($"{defaultMeterColor.r}:{defaultMeterColor.g}:{defaultMeterColor.b}:{defaultMeterColor.a}");
            writer.WriteLine($"{defaultMeterBgColor.r}:{defaultMeterBgColor.g}:{defaultMeterBgColor.b}:{defaultMeterBgColor.a}");

            string colorKeys = "";
            foreach (var item in defaultMeterGradient.colorKeys)
            {
                colorKeys += $"{item.color.r}:{item.color.g}:{item.color.b}:{item.color.a}:{item.time}|";
            }
            colorKeys = colorKeys.TrimEnd('|');
            writer.WriteLine(colorKeys);

            string alphaKeys = "";
            foreach (var item in defaultMeterGradient.alphaKeys)
            {
                alphaKeys += $"{item.alpha}:{item.time}|";
            }
            alphaKeys = alphaKeys.TrimEnd('|');
            writer.WriteLine(alphaKeys);
            writer.WriteLine(defaultMeterGradient.mode);

            writer.Close();
        }

        private static void LoadOptions()
        {
            StreamReader reader = new(path);
            showAnyDefault = bool.Parse(reader.ReadLine());
            showAllDefault = bool.Parse(reader.ReadLine());

            string defaultColorString = reader.ReadLine();
            string defaultBgString = reader.ReadLine();
            string defaultGradientColorKeyString = reader.ReadLine();
            string defaultGradientAlphaKeyString = reader.ReadLine();
            string defaultGradientMode = reader.ReadLine();

            reader.Close();

            float[] defaultColorRBGA = ConvertStringToFloatArray(defaultColorString.Split(':'));
            float[] defaultBGRBGA = ConvertStringToFloatArray(defaultBgString.Split(':'));
            defaultMeterColor = new(defaultColorRBGA[0], defaultColorRBGA[1], defaultColorRBGA[2], defaultColorRBGA[3]);
            defaultMeterBgColor = new(defaultBGRBGA[0], defaultBGRBGA[1], defaultBGRBGA[2], defaultBGRBGA[3]);

            Gradient newGradient = new();
            newGradient.mode = (GradientMode)Enum.Parse(typeof(GradientMode), defaultGradientMode);
            int i = 0;
            string[] defColKeySplit = defaultGradientColorKeyString.Split('|');
            GradientColorKey[] colKeys = new GradientColorKey[defColKeySplit.Length];
            foreach (var defColKey in defColKeySplit)
            {
                float[] defColKeyValues = ConvertStringToFloatArray(defColKey.Split(':'));
                Color col = new(defColKeyValues[0], defColKeyValues[1], defColKeyValues[2], defColKeyValues[3]);
                colKeys[i] = new(
                    col,
                    defColKeyValues[4]);
                i++;
            }

            string[] defAlpKeySplit = defaultGradientAlphaKeyString.Split('|');
            GradientAlphaKey[] alpKeys = new GradientAlphaKey[defAlpKeySplit.Length];
            i = 0;
            foreach (var defAlpKey in defAlpKeySplit)
            {
                float[] defAlpKeyValues = ConvertStringToFloatArray(defAlpKey.Split(':'));
                alpKeys[i] = new(defAlpKeyValues[0], defAlpKeyValues[1]);
                i++;
            }
            //defaultMeterGradient.colorKeys = colKeys;
            //defaultMeterGradient.alphaKeys = alpKeys;
            newGradient.SetKeys(colKeys, alpKeys);
            //Debug.Log(newGradient.colorKeys.Length + "Colors");
            //Debug.Log(newGradient.alphaKeys.Length + "Aplha");
            defaultMeterGradient = newGradient;
        }


        private static float[] ConvertStringToFloatArray(string[] inArray)
        {
            float[] outArray = new float[inArray.Length];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = float.Parse(inArray[i]);
            }
            return outArray;
        }
    }

}