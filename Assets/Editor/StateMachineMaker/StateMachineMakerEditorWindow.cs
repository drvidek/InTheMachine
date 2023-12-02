using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QKit
{
    public class StateMachineMakerEditorWindow : EditorWindow
    {
        [MenuItem("QKit/State Machine Maker")]
        public static void ShowWindow()
        {
            thisWindow = GetWindow(typeof(StateMachineMakerEditorWindow), false, "State Machine Maker");
            windowRect = thisWindow.position;
        }

        ScriptableObject target;
        SerializedObject thisSerialised;
        private void OnEnable()
        {
            target = this;
            thisSerialised = new SerializedObject(target);
        }

        private static Rect windowRect;
        private static EditorWindow thisWindow;

        private Vector2 windowScrollPos;

        public string stateName = "State";
        public string[] stateOptions = new string[0];
        public bool useFixedDelta;

        public string result;

        private void OnGUI()
        {
            if (thisWindow == null)
                thisWindow = GetWindow(typeof(StateMachineMakerEditorWindow));
            if (windowRect.size != thisWindow.position.size)
            {
                windowRect = thisWindow.position;
            }

            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.Width(windowRect.width));

            EditorGUI.BeginChangeCheck();
            stateName = EditorGUILayout.TextField(new GUIContent("Enum Name", "The name to be given to the overall state behaviours"), stateName);

            var options = thisSerialised.FindProperty("stateOptions");

            if (EditorGUI.EndChangeCheck())
            {
                thisSerialised = new SerializedObject(target);
            }

            EditorGUILayout.PropertyField(options, new GUIContent("State Names", "The name to be given to each state and associated coroutine"), true);

            bool stateOptionsInvalid = false;

            foreach (var item in stateOptions)
            {
                if (item == "")
                {
                    stateOptionsInvalid = true;
                    break;
                }
            }

            EditorGUI.BeginDisabledGroup(stateOptions.Length == 0 || stateName == "" || stateOptionsInvalid);

            if (GUILayout.Button(new GUIContent("Generate Machine", "Generates the state machine template using the provided values")))
            {
                result = GenerateStateMachine();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.TextArea(result);

            thisSerialised.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }

        private string GenerateStateMachine()
        {
            string result = "";

            string stateEnum = $"public enum {stateName} " + "{";
            foreach (var state in stateOptions)
            {
                stateEnum += $"{state}, ";
            }
            stateEnum = stateEnum.Remove(stateEnum.Length - 2);
            stateEnum += "}";

            string stateVariable = $"private {stateName} _currentState;";

            string inputVariables =
                "private Vector2 _targetVelocity;\n" +
                "private struct UserInput\r\n    {\r\n        public Vector2 direction;\r\n        public bool jump;\r\n        public bool action;\r\n    }\r\n" +
                "private Vector2 _userInputDir;\r\n    private bool[] _userInputAction = new bool[2];\n private UserInput lastInput;\n" +
                "private void SetNewInputs()\r\n    {\r\n        UserInput inputs = new();\r\n        inputs.direction = _userInputDir;\r\n        inputs.jump = _userInputAction[0];\r\n        inputs.action = _userInputAction[1];\r\n\r\n        lastInput = inputs;\r\n        _userInputAction = new bool[2] { Input.GetButton(\"Jump\"), Input.GetButton(\"Action\") };\r\n    }";

            string statePropery = $"public {stateName} CurrentState => _currentState;";

            string inputProperties =
                "public Vector2 UserInputDir => _userInputDir;\n" +
                "public bool JumpPress => _userInputAction[0] && !lastInput.jump;\r\n" +
                "public bool JumpHold => _userInputAction[0] && lastInput.jump;\r\n" +
                "public bool JumpRelease => !_userInputAction[0] && lastInput.jump;\r\n" +
                "public bool ActionPress => _userInputAction[1] && !lastInput.action;\r\n" +
                "public bool ActionHold => _userInputAction[1] && lastInput.action;\r\n" +
                "public bool ActionRelease => !_userInputAction[1] && lastInput.action;";

            string fixedTime = "private WaitForFixedUpdate waitForFixedUpdate = new();";

            string startMethod =
               "private void Start()\n" +
                "{\n" +
                "   NextState();\n" +
                "}";

            string updateMethod =
                "private void Update()\r\n    " +
                "{\r\n        _userInputAction[0] = Input.GetButton(\"Jump\") || _userInputAction[0];" +
                "\r\n        _userInputAction[1] = Input.GetButton(\"Action\") || _userInputAction[1];" +
                "\r\n        _userInputDir = new(Input.GetAxis(\"Horizontal\"), Input.GetAxis(\"Vertical\"));" +
                "}";

            string fixedUpdateMethod =
                "private void FixedUpdate()\r\n{\r\n" +
                "_rigidbody.velocity = _targetVelocity * (1f / Time.fixedDeltaTime);\r\n" +
                "_targetVelocity = new();\n}";

            string nextStateMethod =
                "/// <summary>\n" +
                "/// Triggers the next state behaviour based on _currentState\n" +
                "/// </summary>\n" +
                "private void NextState()\n" +
                "{\n" +
                "   //start the state coroutine based on the name of our _currentState enum\n" +
                "   StartCoroutine(_currentState.ToString());\n" +
                "}";

            string changeStateMethod =
                "/// <summary>\n" +
                "/// Changes the state, triggering the current state's exit behaviour and the new state's entry behaviour\n" +
                "/// </summary>\n" +
                $"private void ChangeStateTo({stateName} state)\n" +
                "{\n" +
                $"   _currentState = state;\n" +
                "}";

            string stateActions = "";

            string stateCoroutines = "";
            foreach (var state in stateOptions)
            {
                stateActions += $"public Action on{state}Enter;\n" +
                                    $"public Action on{state}Stay;\n" +
                                    $"public Action on{state}Exit;\n";
                string stateCurrent =
                    $"IEnumerator {state}()\n" +
                    "{\n" +
                    "   //on entry\n" +
                    $"   On{state}Enter();\n" +
                    $"   on{state}Enter?.Invoke();\n" +
                    "   //every frame while we're in this state\n" +
                    $"   while (_currentState == {stateName}.{state})\n" +
                    "   {\n" +
                    "       //state behaviour here\n" +
                    "       \n" +
                    $"   On{state}Stay();\n" +
                    $"   on{state}Stay?.Invoke();\n" +

                    "       //wait a frame\n" +
                    "       yield return waitForFixedUpdate;\n" +
                    "   }\n" +
                    "   //on exit\n" +
                    $"   On{state}Exit();\n" +
                    $"   on{state}Exit?.Invoke();\n" +
                    "   //trigger the next state\n" +
                    "   NextState();\n" +
                    "}\n" +
                    "\n" +
                    $"/// <summary>\r\n    /// Called once when entering {state} state\r\n    /// </summary>\n" +
                    $"private void On{state}Enter()\n" +
                    "{\n" +
                    "\n" +
                    "}\n" +
                    "\n" +
                    $"/// <summary>\r\n    /// Called every fixed update when in {state} state\r\n    /// </summary>\n" +
                    $"private void On{state}Stay()\n" +
                    "{\n" +
                    "       //set our next state to...\n" +
                    $"       {stateName} nextState =\n" +
                    "           //stay as we are\n" +
                    "           _currentState;\n" +
                    "       //trigger the next state\n" +
                    "       ChangeStateTo(nextState);\n" +
                    "       SetNewInputs();\n" +
                    "}\n" +
                    "\n" +
                    $"/// <summary>\r\n    /// Called once when exiting {state} state\r\n    /// </summary>\n" +
                    $"private void On{state}Exit()\n" +
                    "{\n" +
                    "\n" +
                    "}\n";

                stateCoroutines += stateCurrent;
            }
            result =
                $"{stateEnum}\n" +
                $"{stateVariable}\n" +
                $"[SerializeField] protected Rigidbody _rigidbody;\n" +
                $"{inputVariables}\n" +
                $"{stateActions}\n" +
                $"{fixedTime}\n" +
                $"{statePropery}\n" +
                $"{inputProperties}\n" +
                $"\n" +
                $"{startMethod}\n" +
                $"{updateMethod}\n" +
                $"{fixedUpdateMethod}\n" +
                $"\n" +
                $"#region State Machine\n" +
                $"{nextStateMethod}\n" +
                $"\n" +
                $"{changeStateMethod}\n" +
                $"\n" +
                $"{stateCoroutines}" +
                $"#endregion";
            return result;
        }

    }

}