using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        List<Renderer> renderers = new();
        foreach (string GUID in AssetDatabase.FindAssets("t:gameobject"))
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(GUID);

            foreach (Renderer spriteRenderer in asset.GetComponentsInChildren<Renderer>())
                renderers.Add(spriteRenderer);
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer.material == source)
                renderer.material = replace;
        }

        AssetDatabase.SaveAssets();
    }

}
