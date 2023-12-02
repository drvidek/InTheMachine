using UnityEngine;
using UnityEditor;

namespace QKit
{

    public class QKitUserGuide : EditorWindow
    {
        [MenuItem("QKit/User Guide (external)")]
        public static void ShowWindow()
        {
            Application.OpenURL("https://docs.google.com/document/d/1ITw1vIcMY0M2qx1VfK6aF-b4V-v_cfkRf9C_CecOdgI/");
        }
    }
}