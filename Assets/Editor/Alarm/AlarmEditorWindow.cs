using System.IO;
using UnityEngine;
using UnityEditor;

namespace QKit
{
    public class AlarmEditorWindow : EditorWindow
    {
        [MenuItem("QKit/Alarms")]
        public static void ShowWindow()
        {
            thisWindow = GetWindow(typeof(AlarmEditorWindow), false, "Alarms");
            windowRect = thisWindow.position;

        }

        public static bool showAllDefault;
        public static bool lockActionButtons = false;
        public static bool disableAlarmChanges = false;
        public bool allowReleaseAll = false;
        public int alarmIndex;

        public Vector2 windowScrollPos;
        public Vector2 allAlarmScrollPos;

        public static EditorWindow thisWindow;
        public static Rect windowRect;

        private static string path = Path.Combine(Application.streamingAssetsPath, "QKitResources/Editor/Alarm.txt");

        private void OnEnable()
        {
            if (File.Exists(path))
                LoadOptions();
        }

        private void OnGUI()
        {
            if (thisWindow == null)
                thisWindow = GetWindow(typeof(AlarmEditorWindow));
            if (windowRect.size != thisWindow.position.size)
            {
                windowRect = thisWindow.position;
            }

            bool playing = Application.isPlaying;
            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            #region Options
            EditorGUILayout.LabelField("Alarm Options", EditorStyles.boldLabel);
            showAllDefault = EditorGUILayout.ToggleLeft("Show extended alarm options in the inspector by default", showAllDefault);
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            AlarmPool.maxAlarmsAllowedInPool = EditorGUILayout.IntField("Max alarms held in pool:", AlarmPool.maxAlarmsAllowedInPool);
            EditorGUILayout.LabelField("Decimal places to display:");
            AlarmForPool.alarmPrecision = EditorGUILayout.IntSlider(AlarmForPool.alarmPrecision, 0, 6);
            lockActionButtons = EditorGUILayout.ToggleLeft("Lock alarm action buttons", lockActionButtons);
            disableAlarmChanges = EditorGUILayout.ToggleLeft("Disable changes to alarms", disableAlarmChanges);
            EditorGUILayout.Space();
            #endregion

            if (!playing)
                allowReleaseAll = false;

            #region Play mode only
            EditorGUILayout.LabelField("Current Alarms", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(!playing);

            #region Alarm Selection
            string[] alarms = new string[AlarmPool.AlarmsInUse.Count + 1];
            alarms[0] = "All";
            for (int i = 0; i < AlarmPool.AlarmsInUse.Count; i++)
            {
                alarms[i + 1] = AlarmPool.AlarmsInUse[i].Label;
            }

            if (alarmIndex > AlarmPool.AlarmsInUse.Count)
                alarmIndex = 0;

            alarmIndex = EditorGUILayout.Popup(alarmIndex, alarms);
            #endregion

            AlarmForPool alarm;

            float width = windowRect.width * 0.95f;

            switch (alarmIndex)
            {
                case 0:
                    #region Action All buttons
                    EditorGUI.BeginDisabledGroup(lockActionButtons);

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(windowRect.width * 0.98f));
                    if (GUILayout.Button("Play All"))
                    {
                        AlarmPool.PlayAll();
                    }
                    if (GUILayout.Button("Pause All"))
                    {
                        AlarmPool.PauseAll();
                    }
                    if (GUILayout.Button("Stop All"))
                    {
                        AlarmPool.StopAll();
                    }
                    if (GUILayout.Button("Reset All"))
                    {
                        AlarmPool.ResetAll();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();
                    #endregion

                    #region Draw alarm fields
                    allAlarmScrollPos = EditorGUILayout.BeginScrollView(allAlarmScrollPos, GUILayout.MaxHeight(windowRect.height / 2));
                    for (int i = 0; i < AlarmPool.AlarmsInUse.Count; i++)
                    {
                        alarm = AlarmPool.AlarmsInUse[i];
                        EditorGUILayout.Space();
                        AlarmDisplay(alarm, width);
                        EditorGUILayout.Space();

                    }
                    EditorGUILayout.EndScrollView();
                    #endregion

                    break;
                default:
                    int index = alarmIndex - 1;
                    alarm = AlarmPool.AlarmsInUse[index];
                    AlarmDisplay(alarm, width);
                    for (int i = 0; i < 10; i++)
                    {
                        EditorGUILayout.Space();

                    }
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug options", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            #region Release All
            EditorGUILayout.BeginHorizontal();
            allowReleaseAll = EditorGUILayout.ToggleLeft("Allow Release All", allowReleaseAll);

            EditorGUI.BeginDisabledGroup(!allowReleaseAll);
            if (GUILayout.Button("Release All"))
            {
                AlarmPool.ReleaseAll();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUI.EndDisabledGroup();
            #endregion
            EditorGUILayout.EndScrollView();
            Repaint();
        }

        private void AlarmDisplay(AlarmForPool alarm, float width)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(width));

            EditorGUILayout.LabelField(alarm.Label, GUILayout.Width(width / 3));
            EditorGUI.BeginDisabledGroup(lockActionButtons);
            if (!alarm.IsPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    alarm.Play();
                }
            }
            else
                if (GUILayout.Button("Pause"))
            {
                alarm.Pause();
            }
            if (GUILayout.Button("Stop"))
            {
                alarm.Stop();
            }
            if (GUILayout.Button("Reset"))
            {
                alarm.Reset();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(disableAlarmChanges);

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
            EditorGUI.BeginChangeCheck();
            float timeRemaining = EditorGUILayout.FloatField(alarm.TimeRemainingClipped);
            GUILayout.Label("of");
            float timeMax = EditorGUILayout.FloatField(alarm.TimeMaxClipped);
            GUILayout.Label("secs remaining");
            GUILayout.EndHorizontal();

            Rect toggleGroup = EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width), GUILayout.ExpandWidth(true));
            bool looping = EditorGUILayout.ToggleLeft("Looping", alarm.Looping, GUILayout.Width(width / 4));
            EditorGUILayout.Separator();
            if (GUILayout.Button("Release", GUILayout.Width(width / 4)))
            {
                AlarmPool.Release(alarm);
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                alarm.SetTimeRemaining(timeRemaining);
                alarm.SetTimeMaximum(timeMax);
            }
            EditorGUI.EndDisabledGroup();

        }

        private void OnDestroy()
        {
            SaveOptions();
        }

        private void SaveOptions()
        {


            StreamWriter writer = new(path, false);
            writer.WriteLine(showAllDefault);
            writer.WriteLine(AlarmForPool.alarmPrecision);
            writer.WriteLine(AlarmPool.maxAlarmsAllowedInPool);
            writer.Close();
        }

        private static void LoadOptions()
        {
            StreamReader reader = new(path);
            string load;
            if ((load = reader.ReadLine()) != "")
                showAllDefault = bool.Parse(load);
            if ((load = reader.ReadLine()) != "")
                AlarmForPool.alarmPrecision = int.Parse(load);
            if ((load = reader.ReadLine()) != "")
                AlarmPool.maxAlarmsAllowedInPool = int.Parse(load);

            reader.Close();

        }
    }

}