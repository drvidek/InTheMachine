using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QKit
{

    public class HighscoreKeeperEditorWindow : EditorWindow
    {
        [MenuItem("QKit/HighscoreKeeper")]
        public static void ShowWindow()
        {
            thisWindow = GetWindow(typeof(HighscoreKeeperEditorWindow), false, "HighscoreKeeper");
            windowRect = thisWindow.position;
        }


        ScriptableObject target;
        SerializedObject thisSerialised;

        private static Rect windowRect;
        private static EditorWindow thisWindow;

        private static string path = Path.Combine(Application.streamingAssetsPath, "QKitResources/Editor/HighscoreKeeper.txt");


        private Vector2 windowScrollPos;
        private static int tempMaxEntriesToKeep;

        public List<string> defaultNames = new();
        public List<float> defaultScores = new();

        bool allowDefault = false;

        private void OnEnable()
        {
            if (File.Exists(path))
                LoadOptions();
            HighscoreKeeper.LoadEntries();
            tempMaxEntriesToKeep = HighscoreKeeper.maxEntriesToKeep;

            ReadDefaultEntries();

            target = this;
            thisSerialised = new SerializedObject(target);
        }


        private void OnGUI()
        {
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            if (thisWindow == null)
                thisWindow = GetWindow(typeof(HighscoreKeeperEditorWindow));
            if (windowRect.size != thisWindow.position.size)
            {
                windowRect = thisWindow.position;
            }

            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            tempMaxEntriesToKeep = EditorGUILayout.IntField("Maximum entries to keep", tempMaxEntriesToKeep);

            if (GUILayout.Button("Confirm"))
            {
                HighscoreKeeper.maxEntriesToKeep = tempMaxEntriesToKeep;
                HighscoreKeeper.SaveEntries();
                HighscoreKeeper.LoadEntries();
            }
            EditorGUILayout.EndHorizontal();

            // EditorGUILayout.LabelField("Save File path:");
            // EditorGUIUtility.labelWidth = stringWidth;
            // HighscoreKeeper.filePath = EditorGUILayout.TextField("/Assets/StreamingAssets", HighscoreKeeper.filePath);
            // EditorGUIUtility.labelWidth = defaultLabelWidth;
            //
            // EditorGUILayout.LabelField("Default File path:");
            // EditorGUIUtility.labelWidth = stringWidth;
            // HighscoreKeeper.defaultFilePath = EditorGUILayout.TextField("/Assets/StreamingAssets", HighscoreKeeper.defaultFilePath);
            // EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Current high scores", EditorStyles.boldLabel);
            for (int i = 0; i < HighscoreKeeper.maxEntriesToKeep; i++)
            {
                string displayString = i >= HighscoreKeeper.Entries.Count ? $"{i + 1}." :
                    $"{i + 1}. {HighscoreKeeper.Entries[i].Key}: {HighscoreKeeper.Entries[i].Value}";
                EditorGUILayout.LabelField(displayString);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!allowDefault);
            if (GUILayout.Button("Reset current highscores to default"))
            {
                HighscoreKeeper.ResetEntriesToDefault();
            }
            EditorGUI.EndDisabledGroup();
            allowDefault = EditorGUILayout.ToggleLeft("Allow default", allowDefault);
            EditorGUILayout.EndHorizontal();

            bool listClip = false;

            while (defaultNames.Count > HighscoreKeeper.maxEntriesToKeep)
            {
                defaultNames.RemoveAt(defaultNames.Count - 1);
                listClip = true;
            }
            while (defaultScores.Count > HighscoreKeeper.maxEntriesToKeep)
            {
                defaultScores.RemoveAt(defaultScores.Count - 1);
                listClip = true;
            }
            while (defaultNames.Count < HighscoreKeeper.maxEntriesToKeep)
            {
                defaultNames.Add("");
                listClip = true;
            }
            while (defaultScores.Count < HighscoreKeeper.maxEntriesToKeep)
            {
                defaultScores.Add(0);
                listClip = true;
            }
            if (listClip)
            {
                ReadDefaultEntries();
                thisSerialised = new SerializedObject(target);
            }

            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Default highscores", EditorStyles.boldLabel);

            SerializedProperty defaultNamesProperty = thisSerialised.FindProperty("defaultNames");
            SerializedProperty defaultScoresProperty = thisSerialised.FindProperty("defaultScores");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(defaultNamesProperty, true);
            EditorGUILayout.PropertyField(defaultScoresProperty, true);
            EditorGUILayout.EndHorizontal();
            thisSerialised.ApplyModifiedProperties();

            if (GUILayout.Button("Save new default scores"))
            {
                WriteNewDefaultEntries();
                ReadDefaultEntries();
            }

            EditorGUILayout.EndScrollView();
        }

        private void ReadDefaultEntries()
        {
            var defaultEntries = HighscoreKeeper.GetDefaultEntries();
            defaultNames = new(HighscoreKeeper.maxEntriesToKeep);
            defaultScores = new(HighscoreKeeper.maxEntriesToKeep);
            for (int i = 0; i < HighscoreKeeper.maxEntriesToKeep; i++)
            {
                defaultNames.Add((i < defaultEntries.Count ? defaultEntries[i].Key : "-"));
                defaultScores.Add((i < defaultEntries.Count ? defaultEntries[i].Value : 0));
            }
        }

        private void WriteNewDefaultEntries()
        {
            //string save = "";
            //for (int i = 0; i < defaultNames.Count; i++)
            //{
            //    save += $"{defaultNames[i]}:{defaultScores[i]}\n";
            //}
            //File.WriteAllText(HighscoreKeeper.defaultPathResources, save);
            StreamWriter writer = new(HighscoreKeeper.defaultPathStreamingAssets, false);
            for (int i = 0; i < defaultNames.Count; i++)
            {
                writer.WriteLine($"{defaultNames[i]}:{defaultScores[i]}");
            }
            writer.Close();
        }


        private void OnDestroy()
        {
            SaveOptions();
        }

        private void SaveOptions()
        {
            StreamWriter writer = new(path, false);
            writer.WriteLine(HighscoreKeeper.maxEntriesToKeep);

            writer.Close();
        }

        private static void LoadOptions()
        {
            StreamReader reader = new(path);
            string load;
            if ((load = reader.ReadLine()) != "")
                HighscoreKeeper.maxEntriesToKeep = int.Parse(load);

            reader.Close();
        }
    }

}