using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    [SerializeField] private Vector2 size;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float ascendSpeed;
    [SerializeField] private float descendSpeed;
    private Rigidbody2D rb;
    private bool active;

    private void OnValidate()
    {
        SetColliderAndSprite();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetColliderAndSprite();
    }

    private void SetColliderAndSprite()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();

        boxCollider.size = new(size.x - 0.1f, size.y) ;
        sprite.size = size;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * (active ? ascendSpeed : descendSpeed);
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
    }
}
