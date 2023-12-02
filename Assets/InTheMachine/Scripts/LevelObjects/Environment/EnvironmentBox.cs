using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBox : MonoBehaviour
{
    protected Collider2D _collider;
    protected SpriteRenderer sprite;
    [SerializeField] protected Vector2 size;
    // Start is called before the first frame update

    public BoxCollider2D boxCollider => _collider as BoxCollider2D;

    protected void OnValidate()
    {
        SetColliderAndSprite();
    }


    protected virtual void Start()
    {
        SetColliderAndSprite();
    }

    public void SetSize(Vector2 size)
    {
#if UNITY_EDITOR
        this.size = size;
        SetColliderAndSprite();
#endif
    }

    protected virtual void SetColliderAndSprite()
    {
        SetCollider();
        SetSprite();
    }

    protected virtual void SetCollider()
    {
        if (!_collider) _collider = GetComponent<Collider2D>();
        if (_collider is BoxCollider2D)
        boxCollider.size = new(size.x - 0.1f, size.y);
    }

    protected virtual void SetSprite()
    {
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.size = size;

    }
}
