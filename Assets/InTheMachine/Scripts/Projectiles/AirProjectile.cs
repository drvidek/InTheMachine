using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class AirProjectile : Projectile, IFlammable
{
    private float baseSpeed;
    private bool lifeOver;
    public override void ApplyProjectileProperties(Vector3 direction, float size, float speed, float lifetime, float power, LayerMask colliding, LayerMask piercing)
    {
        base.ApplyProjectileProperties(direction, size, speed, lifetime, power, colliding, piercing);
        baseSpeed = speed;
        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Alarm endOfLife = Alarm.GetAndPlay(_lifetime);
        endOfLife.onComplete = EndOfLife;
        if (Direction.x == 0)
        {
            sprite.flipY = PlayerAnimate.main.FacingDirection.x < 0;
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
            sprite.flipX = PlayerAnimate.main.FacingDirection.x < 0;
    }

    private void FixedUpdate()
    {
        if (lifeOver)
            return;
        _speed -= (baseSpeed / _lifetime) * Time.fixedDeltaTime;
        rb.velocity = Direction * Speed;
    }

    protected override void DoCollision(Collider2D collider)
    {
        //if we hit a flammable object
        if (collider.TryGetComponent<IFlammable>(out IFlammable flame))
        {
            //if it's flaming
            if (flame.IsFlaming())
            {
                //douse it and destroy the shot
                flame.DouseFlame();
                base.DoCollision(collider);
                return;
            }
            //otherwise, if it's not an enemy, don't destroy the shot
            if (!collider.GetComponent<EnemyMachine>())
                return;
        }
        base.DoCollision(collider);
    }

    protected override void EndOfLife()
    {
        lifeOver = true;
        rb.simulated = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 2f);
    }

    public void PropagateFlame(Collider2D collider)
    {

    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public void CatchFlame(Collider2D collider)
    {

    }

    public void DouseFlame()
    {

    }

    public bool IsFlaming()
    {
        return false;
    }
}
