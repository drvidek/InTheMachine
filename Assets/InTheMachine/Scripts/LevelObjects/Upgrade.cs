using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private Player.AbilityType ability;
    [SerializeField] private Sprite sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D _collider;

    public Player.AbilityType Ability => ability;

    private void OnValidate()
    {
        SetSprite();
    }

    private void Start()
    {
        SetSprite();
        _collider = GetComponent<Collider2D>();
    }

    public void Collect()
    {
        spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    private void SetSprite()
    {
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
    }
}
