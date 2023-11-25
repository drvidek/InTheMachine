using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBox : MonoBehaviour
{
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;
    [SerializeField] protected Vector2 size;
    // Start is called before the first frame update

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

    protected void SetColliderAndSprite()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();

        boxCollider.size = new(size.x - 0.1f, size.y);
        sprite.size = size;
    }
}
