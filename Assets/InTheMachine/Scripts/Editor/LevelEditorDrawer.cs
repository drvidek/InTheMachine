using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using QKit;

[CustomEditor(typeof(LevelEditor))]
[CanEditMultipleObjects]
public class LevelEditorDrawer : Editor
{
    SerializedProperty placing;



    private void OnEnable()
    {
        placing = serializedObject.FindProperty("placing");

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        LevelEditor editor = (LevelEditor)target;

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        Event e = Event.current;
        bool click = e.type == EventType.MouseDown && e.button == 0;

        Vector3Int relativeCell = editor.interactibleGrid.WorldToCell(ray.origin);

        Vector3 cellPosition = editor.interactibleGrid.CellToWorld(relativeCell);

        editor.currentSprite.transform.position = cellPosition;

        if (click && SceneView.focusedWindow)
        {
            SpawnObject(editor, cellPosition);
        }

        Handles.BeginGUI();

        Handles.EndGUI();
    }

    private GameObject SpawnObject(LevelEditor editor, Vector3 position)
    {
        GameObject go = null;
        switch (editor.placing)
        {
            case LevelEditor.Placing.Environment:
                break;
            case LevelEditor.Placing.Toggles:
                break;
            case LevelEditor.Placing.Debris:
                go = editor.debris.prefab;
                break;
            default:
                break;
        }
        if (go == null)
            return null;

        GameObject objInScene = PrefabUtility.InstantiatePrefab(go, editor.interactibleGrid.transform) as GameObject;

        objInScene.transform.position = position;

        PrefabUtility.RecordPrefabInstancePropertyModifications(objInScene);

        return objInScene;
    }

    private void SetDebrisFromRandom(GameObject debris, Sprite[] sprites)
    {
        bool onGround = Physics2D.BoxCast(debris.transform.position, Vector3.one, 0, Vector3.down, 0.02f);
        bool leftWall = Physics2D.BoxCast(debris.transform.position, Vector3.one, 0, Vector3.left, 0.02f);
        bool rightWall = Physics2D.BoxCast(debris.transform.position, Vector3.one, 0, Vector3.right, 0.02f);

        if (onGround)
        {
            if (leftWall || rightWall)
            {

            }
        }
    }
}
