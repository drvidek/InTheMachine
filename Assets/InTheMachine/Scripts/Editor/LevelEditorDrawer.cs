using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Tilemaps;
using QKit;
using System;
using UnityEditor.Events;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorDrawer : Editor
{
    bool showUnderTheHood;
    public bool newClick = true;

    bool paint = false;

    SerializedProperty currentCategory;

    SerializedProperty currentToggleType;
    SerializedProperty selectObjectsToActivate;
    SerializedProperty objectsToActivate;

    SerializedProperty doorLength;
    SerializedProperty doorRotation;
    SerializedProperty selectToggleToUse;
    SerializedProperty toggleToUse;
    SerializedProperty parentTransformInScene;
    SerializedProperty environmentType;
    SerializedProperty blockSize;


    public enum CobwebDir
    {
        BL = 2,
        TL,
        TR,
        BR
    }

    private void OnEnable()
    {
        GameObject go = (target as LevelEditor).gameObject;
        go.SetActive(true);
        //initialise fields
        currentCategory = serializedObject.FindProperty("currentCategory");
        currentToggleType = serializedObject.FindProperty("currentToggleType");
        selectObjectsToActivate = serializedObject.FindProperty("selectObjectsToActivate");
        objectsToActivate = serializedObject.FindProperty("objectsToActivate");
        doorLength = serializedObject.FindProperty("doorLength");
        doorRotation = serializedObject.FindProperty("doorRotation");
        selectToggleToUse = serializedObject.FindProperty("selectToggleToUse");
        toggleToUse = serializedObject.FindProperty("toggleToUse");
        parentTransformInScene = serializedObject.FindProperty("parentTransformInScene");
        environmentType = serializedObject.FindProperty("environmentType");
        blockSize = serializedObject.FindProperty("blockSize");

    }

    private void OnDisable()
    {
        (target as LevelEditor).gameObject.SetActive(false);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LevelEditor editor = (LevelEditor)target;

        EditorGUILayout.PropertyField(currentCategory);
        EditorGUILayout.PropertyField(parentTransformInScene);

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Toggles:
                EditorGUILayout.PropertyField(currentToggleType);
                EditorGUILayout.PropertyField(selectObjectsToActivate);
                if (selectObjectsToActivate.boolValue)
                {
                    EditorGUILayout.PropertyField(objectsToActivate);
                }
                break;
            case LevelEditor.Category.Player:
                break;
            case LevelEditor.Category.Enemy:
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Debris:
                break;
            case LevelEditor.Category.Environment:
                EditorGUILayout.PropertyField(environmentType);
                EditorGUILayout.PropertyField(blockSize);
                break;
            case LevelEditor.Category.Door:
                EditorGUILayout.PropertyField(doorLength);
                EditorGUILayout.PropertyField(doorRotation);
                EditorGUILayout.PropertyField(selectToggleToUse);
                if (selectToggleToUse.boolValue)
                {
                    EditorGUILayout.PropertyField(toggleToUse);
                }
                break;
            default:
                break;
        }

        if (target)
        {
            serializedObject.ApplyModifiedProperties();
        }


        EditorGUILayout.Space();
        showUnderTheHood = EditorGUILayout.Toggle("Show Under The Hood", showUnderTheHood);
        if (showUnderTheHood)
            base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        LevelEditor editor = (LevelEditor)target;

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
            paint = true;
        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl)
            paint = false;

        bool click = (e.type == EventType.MouseDown && e.button == 0 && (newClick || paint));

        Vector3Int relativeCell = editor.interactibleGrid.WorldToCell(ray.origin);

        Vector3 cellPosition = editor.interactibleGrid.CellToWorld(relativeCell);

        LevelEditor.currentPosition = cellPosition;
        Vector2 wallDir = CheckOnNeighbouringCells(cellPosition, Vector2.one, (int)LevelEditor.Category.Environment);
        bool toggleHere = CheckForOverlap(cellPosition, Vector2.one, (int)LevelEditor.Category.Toggles);
        editor.spritePreview.transform.parent.position = cellPosition;
        editor.spritePreview.transform.position = cellPosition;
        editor.spritePreview.transform.parent.localEulerAngles = Vector3.zero;
        editor.spritePreview.drawMode = SpriteDrawMode.Simple;
        editor.spritePreview.transform.localScale = Vector3.one;
        editor.spritePreview.size = Vector2.one;

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Toggles:

                editor.spritePreview.sprite = editor.levelToggles[currentToggleType.intValue].GetComponentInChildren<SpriteRenderer>().sprite;
                break;
            case LevelEditor.Category.Player:
                break;
            case LevelEditor.Category.Enemy:
                break;
            case LevelEditor.Category.PhysicsObject:
                editor.spritePreview.sprite = editor.physicsObject.GetComponentInChildren<SpriteRenderer>().sprite;

                break;
            case LevelEditor.Category.Debris:
                editor.spritePreview.sprite =
                    toggleHere || (wallDir.y == -1 && wallDir.x == 0) ? editor.debris.sprites[^1] :
                    editor.cobweb.sprites[0];
                break;
            case LevelEditor.Category.Environment:
                editor.spritePreview.sprite = editor.environmentPrefabs[(int)editor.environmentType].GetComponentInChildren<SpriteRenderer>().sprite;
                editor.spritePreview.drawMode = SpriteDrawMode.Tiled;
                editor.spritePreview.size = new(editor.blockSize.x, editor.blockSize.y);
                editor.spritePreview.tileMode = SpriteTileMode.Adaptive;
                editor.spritePreview.transform.localScale = Vector3.one;

                break;
            case LevelEditor.Category.Door:
                editor.spritePreview.sprite = editor.door.GetComponentInChildren<SpriteRenderer>().sprite;
                editor.spritePreview.drawMode = SpriteDrawMode.Tiled;
                editor.spritePreview.size = new(1, editor.doorLength);
                editor.spritePreview.transform.parent.localEulerAngles = new Vector3(0, 0, editor.doorRotation);
                editor.spritePreview.transform.localPosition = Vector3.down * (editor.doorLength / 2f - 0.5f);
                editor.spritePreview.transform.localScale = Vector3.one;

                break;
            default:
                break;
        }

        if (click && SceneView.focusedWindow)
        {
            SpawnObject(editor, cellPosition);
            newClick = false;
        }
        else
            newClick = true;

    }


    private GameObject SpawnObject(LevelEditor editor, Vector3 position)
    {
        GameObject go = null;
        Collider2D debris = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Debris);
        Collider2D toggle = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Toggles);
        Collider2D block = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Environment);
        Vector2 walls = CheckOnNeighbouringCells(position, Vector2.one, (int)LevelEditor.Category.Environment);

        if (block)
            return null;

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Environment:
                go = editor.environmentPrefabs[(int)editor.environmentType];
                break;
            case LevelEditor.Category.Toggles:
                if (toggle)
                    break;
                if (editor.currentToggleType == LevelEditor.ToggleType.Button && walls.y != -1)
                    break;
                go = editor.levelToggles[(int)editor.currentToggleType];
                break;
            case LevelEditor.Category.Debris:
                if (debris)
                    break;
                if (toggle && toggle.GetComponent<Button>())
                {
                    go = editor.debris.prefab;
                }
                else
                {
                    if (walls.x == 0 && walls.y == -1)
                        go = editor.debris.prefab;
                    else
                        go = editor.cobweb.prefab;
                }
                break;
            case LevelEditor.Category.Player:
                break;
            case LevelEditor.Category.Enemy:
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Door:
                go = editor.door;
                break;
            default:
                break;
        }
        if (go == null)
            return null;

        GameObject obSpawnedjInScene = PrefabUtility.InstantiatePrefab(go, editor.parentTransformInScene != null ? editor.parentTransformInScene : editor.interactibleGrid.transform) as GameObject;

        obSpawnedjInScene.transform.position = position;

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Environment:
                if (QMath.TryGet<EnvironmentBox>(obSpawnedjInScene.transform, out EnvironmentBox box))
                    box.SetSize(editor.blockSize);
                break;
            case LevelEditor.Category.Toggles:
                LevelToggle toggleSpawned = obSpawnedjInScene.GetComponent<LevelToggle>();
                foreach (var objToActivate in editor.objectsToActivate)
                {
                    if (QMath.TryGet<IActivate>(objToActivate.transform, out IActivate activate))
                    {
                        UnityEventTools.AddPersistentListener(toggleSpawned.onActiveChanged, activate.ToggleActive);
                    }
                    PrefabUtility.RecordPrefabInstancePropertyModifications(toggleSpawned);
                }
                break;
            case LevelEditor.Category.Debris:
                Vector2 wallDir = CheckOnNeighbouringCells(position, Vector2.one, (int)LevelEditor.Category.Environment);
                SpriteRenderer renderer = obSpawnedjInScene.GetComponentInChildren<SpriteRenderer>();
                renderer.sprite =
                    //is there a button? if so, big dust
                    CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Toggles) ? editor.debris.sprites[^1] :
                    //something on 2 sides? figure out the corner web
                    wallDir.y != 0 && wallDir.x != 0 ?
                    (
                        //on the ground?
                        wallDir.y == -1 ? (
                            //to the left or right?
                            wallDir.x == -1 ? editor.cobweb.sprites[(int)CobwebDir.BL] : editor.cobweb.sprites[(int)CobwebDir.BR]
                        ) :
                            //on the ceiling?
                            //to the left or right?
                            wallDir.x == -1 ? editor.cobweb.sprites[(int)CobwebDir.TL] : editor.cobweb.sprites[(int)CobwebDir.TR]
                    ) :
                    //do we have no walls and/or floor? regular cobweb
                    wallDir.y > -1 && wallDir.x == 0 ? QMath.Choose<Sprite>(editor.cobweb.sprites[0], editor.cobweb.sprites[1]) :
                    //else we are on the ground with no corner, or surrounded on more than 2 sides
                    QMath.Choose<Sprite>(editor.debris.sprites);

                renderer.flipX = wallDir.x == 0 && QMath.Choose<bool>(true, false);
                renderer.flipY = wallDir.y == 0 && QMath.Choose<bool>(true, false);
                break;
            case LevelEditor.Category.Player:
                break;
            case LevelEditor.Category.Enemy:
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Door:
                Door door = obSpawnedjInScene.GetComponent<Door>();
                door.SetValues(doorLength.intValue, doorRotation.floatValue);
                door.Initialise();
                if (selectToggleToUse.boolValue)
                {
                    UnityEventTools.AddPersistentListener(editor.toggleToUse.onActiveChanged, door.ToggleActive);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(editor.toggleToUse);
                }
                break;
            default:
                break;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(obSpawnedjInScene);

        return obSpawnedjInScene;
    }

    private Vector2 CheckOnNeighbouringCells(Vector3 position, Vector2 size, int layer)
    {
        int x = 0;
        int y = 0;
        if (Physics2D.BoxCast(position, size * 0.95f, 0, Vector2.left, 0.02f, 1 << layer))
            x--;
        if (Physics2D.BoxCast(position, size * 0.95f, 0, Vector2.right, 0.02f, 1 << layer))
            x++;
        if (Physics2D.BoxCast(position, size * 0.95f, 0, Vector2.up, 0.02f, 1 << layer))
            y++;
        if (Physics2D.BoxCast(position, size * 0.95f, 0, Vector2.down, 0.02f, 1 << layer))
            y--;

        return new(x, y);
    }

    private Collider2D CheckForOverlap(Vector3 position, Vector2 size, int layer)
    {
        Collider2D toggle = Physics2D.OverlapBox(position, size * 0.95f, 0, 1 << layer);
        return toggle;
    }

}
