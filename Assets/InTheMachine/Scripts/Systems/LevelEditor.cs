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
        Fan,
        HeatSensor
    }

    public enum PlayerType
    {
        Ability,
        PowerUp
    }

    public enum EnvironmentType
    {
        OneWay,
        Moving,
        FireSource
    }

    [Header("Main guts")]
    public SpriteRenderer spritePreview;
    public Category currentCategory;
    public Grid environmentGrid, interactibleGrid;
    public float objectRotation;
    public bool startActive;
    public LayerMask groundLayer;

    [Header("Toggles")]
    public ToggleType currentToggleType;
    public GameObject[] levelToggles;
    public bool selectObjectsFromSceneToActivate;
    public bool buttonStayPressed;

    [Header("Doors")]
    public GameObject door;
    public int doorLength;
    public bool selectToggleInSceneToUse;

    [Header("Debris")]
    public PrefabInfo debris;
    public PrefabInfo cobweb;

    [Header("Physics")]
    public GameObject physicsObject;

    [Header("Environment")]
    public EnvironmentType environmentType;
    public GameObject[] environmentPrefabs;
    public Vector2Int blockSize = Vector2Int.one;

    [Header("Player")]
    public PlayerType currentPlayerType;
    public GameObject[] upgradePrefabs;
    public Player.Ability currentAbility;
    public PowerUp.Type currentPowerUp;

    [Header("Enemies")]
    public EnemyList.Type currentEnemy;
    public EnemyList enemyList;

    [Header("Scene objects")]
    public LevelToggle toggleInSceneToUse;
    public Transform parentTransformInScene;
    public GameObject[] objectsInSceneToActivate = new GameObject[0];

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
