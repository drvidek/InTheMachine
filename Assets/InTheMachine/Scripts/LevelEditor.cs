using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{

    [System.Serializable]
    public struct PrefabInfo
    {
        [SerializeField] public GameObject prefab;
        [SerializeField] public Sprite[] sprites;
    }

    public enum Placing
    {
        Environment,
        Toggles,
        Debris
    }

    public SpriteRenderer currentSprite;

    public Placing placing;
    public Grid environmentGrid, interactibleGrid;
    public GameObject[] levelToggles;
    public GameObject door;
    public PrefabInfo debris;

    private void OnValidate()
    {
        currentSprite = GetComponentInChildren<SpriteRenderer>();

    }
}
