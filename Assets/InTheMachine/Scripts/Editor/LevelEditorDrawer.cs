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

    SerializedProperty parentTransformInScene;
    SerializedProperty objectRotation;

    SerializedProperty currentToggleType;
    SerializedProperty objectsInSceneToActivate;
    SerializedProperty selectObjectsFromSceneToActivate;
    SerializedProperty buttonStayPressed;

    SerializedProperty doorLength;
    SerializedProperty selectToggleInSceneToUse;
    SerializedProperty toggleInSceneToUse;

    SerializedProperty environmentType;
    SerializedProperty blockSize;

    SerializedProperty currentEnemy;

    SerializedProperty currentPlayerType;
    SerializedProperty currentAbility;
    SerializedProperty currentPowerUp;


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
        selectObjectsFromSceneToActivate = serializedObject.FindProperty("selectObjectsFromSceneToActivate");
        objectsInSceneToActivate = serializedObject.FindProperty("objectsInSceneToActivate");
        doorLength = serializedObject.FindProperty("doorLength");
        objectRotation = serializedObject.FindProperty("objectRotation");
        selectToggleInSceneToUse = serializedObject.FindProperty("selectToggleInSceneToUse");
        toggleInSceneToUse = serializedObject.FindProperty("toggleInSceneToUse");
        buttonStayPressed = serializedObject.FindProperty("buttonStayPressed");
        parentTransformInScene = serializedObject.FindProperty("parentTransformInScene");
        environmentType = serializedObject.FindProperty("environmentType");
        blockSize = serializedObject.FindProperty("blockSize");
        currentEnemy = serializedObject.FindProperty("currentEnemy");
        currentPlayerType = serializedObject.FindProperty("currentPlayerType");
        currentAbility = serializedObject.FindProperty("currentAbility");
        currentPowerUp = serializedObject.FindProperty("currentPowerUp");

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
                if (editor.currentToggleType == LevelEditor.ToggleType.Button)
                    EditorGUILayout.PropertyField(buttonStayPressed);
                if (editor.currentToggleType != LevelEditor.ToggleType.Fan)
                    EditorGUILayout.PropertyField(objectRotation);
                EditorGUILayout.PropertyField(selectObjectsFromSceneToActivate);
                if (selectObjectsFromSceneToActivate.boolValue)
                {
                    EditorGUILayout.PropertyField(objectsInSceneToActivate);
                }
                break;
            case LevelEditor.Category.Player:
                EditorGUILayout.PropertyField(currentPlayerType);
                switch (editor.currentPlayerType)
                {
                    case LevelEditor.PlayerType.Ability:
                        EditorGUILayout.PropertyField(currentAbility);
                        break;
                    case LevelEditor.PlayerType.PowerUp:
                        EditorGUILayout.PropertyField(currentPowerUp);
                        break;
                    default:
                        break;
                }
                break;
            case LevelEditor.Category.Enemy:
                EditorGUILayout.PropertyField(currentEnemy);
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Debris:
                break;
            case LevelEditor.Category.Environment:
                EditorGUILayout.PropertyField(environmentType);
                if (editor.environmentType < LevelEditor.EnvironmentType.FireSource)
                {
                    EditorGUILayout.PropertyField(blockSize);
                }
                else
                {
                    EditorGUILayout.PropertyField(objectRotation);
                    blockSize.vector2IntValue = Vector2Int.one;
                }
                break;
            case LevelEditor.Category.Door:
                EditorGUILayout.PropertyField(doorLength);
                EditorGUILayout.PropertyField(objectRotation);
                EditorGUILayout.PropertyField(selectToggleInSceneToUse);
                if (selectToggleInSceneToUse.boolValue)
                {
                    EditorGUILayout.PropertyField(toggleInSceneToUse);
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
                if (editor.currentToggleType == LevelEditor.ToggleType.Button)
                    editor.spritePreview.transform.localEulerAngles = new Vector3(0, 0, editor.objectRotation);
                editor.spritePreview.sprite = editor.levelToggles[currentToggleType.intValue].GetComponentInChildren<SpriteRenderer>().sprite;
                break;
            case LevelEditor.Category.Player:
                switch (editor.currentPlayerType)
                {
                    case LevelEditor.PlayerType.Ability:
                editor.spritePreview.sprite = editor.upgradePrefabs[currentPlayerType.intValue].GetComponentInChildren<SpriteRenderer>().sprite;
                        break;
                    case LevelEditor.PlayerType.PowerUp:
                editor.spritePreview.sprite = editor.upgradePrefabs[currentPlayerType.intValue].GetComponentInChildren<SpriteRenderer>().sprite;
                        break;
                    default:
                        break;
                }
                break;
            case LevelEditor.Category.Enemy:
                editor.spritePreview.sprite = editor.enemyList.enemyPrefabs[(int)editor.currentEnemy].GetComponentInChildren<SpriteRenderer>().sprite;
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
                if (editor.environmentType == LevelEditor.EnvironmentType.FireSource)
                {
                    editor.spritePreview.transform.localEulerAngles = new Vector3(0, 0, editor.objectRotation);
                }
                else
                    editor.spritePreview.transform.localEulerAngles = Vector3.zero;


                break;
            case LevelEditor.Category.Door:
                editor.spritePreview.sprite = editor.door.GetComponentInChildren<SpriteRenderer>().sprite;
                editor.spritePreview.drawMode = SpriteDrawMode.Tiled;
                editor.spritePreview.size = new(1, editor.doorLength);
                editor.spritePreview.transform.parent.localEulerAngles = new Vector3(0, 0, editor.objectRotation);
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
        GameObject finalPrefab = null;
        Collider2D debris = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Debris);
        Collider2D toggle = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Toggles);
        Collider2D block = CheckForOverlap(position, Vector2.one, (int)LevelEditor.Category.Environment);
        Vector2 walls = CheckOnNeighbouringCells(position, Vector2.one, (int)LevelEditor.Category.Environment);

        if (block)
            return null;

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Environment:
                finalPrefab = editor.environmentPrefabs[(int)editor.environmentType];
                break;
            case LevelEditor.Category.Toggles:
                if (toggle)
                    break;
                if (editor.currentToggleType == LevelEditor.ToggleType.Button && walls.y != -1)
                    break;
                finalPrefab = editor.levelToggles[(int)editor.currentToggleType];
                break;
            case LevelEditor.Category.Debris:
                if (debris)
                    break;
                if (toggle && toggle.GetComponent<Button>())
                {
                    finalPrefab = editor.debris.prefab;
                }
                else
                {
                    if (walls.x == 0 && walls.y == -1)
                        finalPrefab = editor.debris.prefab;
                    else
                        finalPrefab = editor.cobweb.prefab;
                }
                break;
            case LevelEditor.Category.Player:
                switch (editor.currentPlayerType)
                {
                    case LevelEditor.PlayerType.Ability:
                        finalPrefab = editor.upgradePrefabs[0];
                        break;
                    case LevelEditor.PlayerType.PowerUp:
                        finalPrefab = editor.upgradePrefabs[1];
                        break;
                    default:
                        break;
                }
                break;
            case LevelEditor.Category.Enemy:
                finalPrefab = editor.enemyList.enemyPrefabs[(int)editor.currentEnemy];
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Door:
                finalPrefab = editor.door;
                break;
            default:
                break;
        }
        if (finalPrefab == null)
            return null;

        GameObject objSpawnedjInScene = PrefabUtility.InstantiatePrefab(finalPrefab, editor.parentTransformInScene != null ? editor.parentTransformInScene : editor.interactibleGrid.transform) as GameObject;

        objSpawnedjInScene.transform.position = position;

        switch (editor.currentCategory)
        {
            case LevelEditor.Category.Environment:
                if (QMath.TryGet<EnvironmentBox>(objSpawnedjInScene.transform, out EnvironmentBox box))
                    box.SetSize(editor.blockSize);
                if (editor.environmentType >= LevelEditor.EnvironmentType.FireSource)
                {
                    objSpawnedjInScene.transform.localEulerAngles = new Vector3(0, 0, editor.objectRotation);
                }
                break;
            case LevelEditor.Category.Toggles:
                LevelToggle toggleSpawned = objSpawnedjInScene.GetComponent<LevelToggle>();
                if (buttonStayPressed.boolValue)
                {
                    (toggleSpawned as Button).SetStayPressed();
                }
                if (toggleSpawned is Button)
                    objSpawnedjInScene.transform.localEulerAngles = new Vector3(0, 0, editor.objectRotation);
                foreach (var objToActivate in editor.objectsInSceneToActivate)
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
                SpriteRenderer renderer = objSpawnedjInScene.GetComponentInChildren<SpriteRenderer>();
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
                switch (editor.currentPlayerType)
                {
                    case LevelEditor.PlayerType.Ability:
                        objSpawnedjInScene.GetComponent<AbilityUnlock>().SetType(editor.currentAbility);
                        break;
                    case LevelEditor.PlayerType.PowerUp:
                        objSpawnedjInScene.GetComponent<PowerUp>().SetType(editor.currentPowerUp);
                        break;
                    default:
                        break;
                }
                break;
            case LevelEditor.Category.Enemy:
                if (editor.currentEnemy == EnemyList.Type.Beetle)
                {
                    objSpawnedjInScene.transform.position += Vector3.down * 0.5f;
                }
                break;
            case LevelEditor.Category.PhysicsObject:
                break;
            case LevelEditor.Category.Door:
                Door door = objSpawnedjInScene.GetComponent<Door>();
                door.SetValues(doorLength.intValue, objectRotation.floatValue);
                door.Initialise();
                if (selectToggleInSceneToUse.boolValue)
                {
                    UnityEventTools.AddPersistentListener(editor.toggleInSceneToUse.onActiveChanged, door.ToggleActive);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(editor.toggleInSceneToUse);
                }
                break;
            default:
                break;
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(objSpawnedjInScene);

        return objSpawnedjInScene;
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
