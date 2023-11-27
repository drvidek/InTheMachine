using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{

    public static Vector3 currentPosition;

    [System.Serializable]
    public struct PrefabInfo
    {
        [SerializeField] public GameObject prefab;
        [SerializeField] public Sprite[] sprites;
    }

    public enum Category
    {
        Environment = 10,
        Door = 1,
        Toggles = 2,
        Debris = 9,
        Player = 6,
        Enemy = 7,
        PhysicsObject = 8
    }

    public enum ToggleType
    {
        Button,
        Fan
    }

    public enum EnvironmentType
    {
        OneWay,
        Moving
    }

    [Header("Main guts")]
    public SpriteRenderer spritePreview;
    public Category currentCategory;
    public Grid environmentGrid, interactibleGrid;

    [Header("Toggles")]
    public ToggleType currentToggleType;
    public GameObject[] levelToggles;
    public bool selectObjectsToActivate;

    [Header("Doors")]
    public GameObject door;
    public int doorLength;
    public float doorRotation;
    public bool selectToggleToUse;

    [Header("Debris")]
    public PrefabInfo debris;
    public PrefabInfo cobweb;

    [Header("Physics")]
    public GameObject physicsObject;

    [Header("Environment")]
    public EnvironmentType environmentType;
    public GameObject[] environmentPrefabs;
    public Vector2Int blockSize = Vector2Int.one;


    [Header("Scene objects")]
    public LevelToggle toggleToUse;
    public Transform parentTransformInScene;
    public GameObject[] objectsToActivate = new GameObject[0];
    public bool buttonStayPressed;

    private void OnValidate()
    {
        spritePreview = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new(1, 1, 1, 0.5f);
        Gizmos.DrawCube(currentPosition, Vector3.one);
    }
}
