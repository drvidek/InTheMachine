using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using System.Drawing;
using UnityEngine.UIElements;

public class SporeProjectile : Projectile, IFlammable
{
    [SerializeField] protected float grav, horStopRate, spriteFlipTime = 0.25f;
    protected SpriteRenderer spriteRenderer;
    protected float spriteFlip;
    protected float startHorSpeed;
    protected bool burning;

    protected Collider2D _collider;
    protected GameObject burnEffect;

    protected float health = 0.5f;

    protected override void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<Collider2D>();
        base.Start();
        _direction = _direction * _speed;
        startHorSpeed = Mathf.Abs(_direction.x);
    }

    protected virtual void FixedUpdate()
    {
        spriteRenderer.flipY = Direction.y > 0;
        _direction.y -= grav * Time.fixedDeltaTime;
        _direction.x = Mathf.MoveTowards(_direction.x, 0, startHorSpeed * horStopRate * Time.fixedDeltaTime);

        if (burning)
        {
            health -= Time.fixedDeltaTime;
            if (health <= 0)
            {
                IFlammable.InstantiateSmokePuff(transform);
                Destroy(gameObject);
            }
            PropagateFlame(_collider);
        }

        if (Direction.y < 0)
        {
            spriteFlip += Time.fixedDeltaTime;
            if (spriteFlip > spriteFlipTime)
            {
                spriteFlip = 0;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
        MoveFixed();
    }

    protected override void MoveFixed()
    {
        rb.velocity = _direction;
    }

    public void PropagateFlame(Collider2D collider)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame(collider);
        }
    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(position, size))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame(_collider);
        }
    }

    public virtual void CatchFlame(Collider2D collider)
    {
        if (!burning)
        {

            burning = true;
            burnEffect = Instantiate(IFlammable.psysObjFire, transform);
            Destroy(gameObject, 1f);
        }
    }

    public void DouseFlame()
    {
        burning = false;
        IFlammable.ClearFireAndSmoke(burnEffect);
    }

    public bool IsFlaming()
    {
        return burning;
    }
}
