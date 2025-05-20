using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Drawing.Printing;

public class MaterialSwapper : EditorWindow
{
    [MenuItem("Material Swapper", menuItem = "Machine/Menu Swapper")]
    public static void Open()
    {
        GetWindow<MaterialSwapper>("Material Swapper", true);
    }

    public Material source;

    public Material replace;

    GameObject testPrefab;

    void OnGUI()
    {
        source = (Material)EditorGUILayout.ObjectField(source, typeof(Material), true);
        replace = (Material)EditorGUILayout.ObjectField(replace, typeof(Material), true);

        testPrefab = (GameObject)EditorGUILayout.ObjectField(testPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Do Prefabs"))
        {
            DoPrefabs();
        }

    }

    void DoPrefabs()
    {
        foreach (string GUID in AssetDatabase.FindAssets("t:GameObject"))
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(GUID));

            if (asset)
            {
                foreach (SpriteRenderer renderer in asset.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (renderer.sharedMaterial == source)
                    {
                        Debug.Log($"Replacing material for {asset.name}");
                        renderer.sharedMaterial = replace;
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
    }

}
