using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected ParticleSystem psysSparkle;
    [SerializeField] protected Collider2D _collider;

    protected virtual void OnValidate()
    {
        SetSprite();
    }

    protected virtual void Start()
    {
        SetSprite();
        _collider = GetComponent<Collider2D>();
    }

    public virtual void Collect()
    {
        spriteRenderer.enabled = false;
        _collider.enabled = false;
        psysSparkle.Stop();
    }

    protected  virtual void SetSprite()
    {
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
    }
}
