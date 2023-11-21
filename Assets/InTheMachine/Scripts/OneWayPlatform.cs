using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private Vector2 size = new(3,1);
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;

    private float yPos;

    private float playerHalfHeight;

    private void OnValidate()
    {
        SetCollider();
    }

    private void Start()
    {
        SetCollider();
        SetYPos();
        playerHalfHeight = Player.main.Height / 2;
    }

    private void SetCollider()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
        if (!sprite) sprite = GetComponent<SpriteRenderer>();

        boxCollider.size = sprite.size = size;
    }

    private void SetYPos()
    {
        yPos = transform.position.y + boxCollider.size.y / 2;
    }

    private void FixedUpdate()
    {
        boxCollider.enabled = Player.main.Y - playerHalfHeight > yPos;
    }
}
