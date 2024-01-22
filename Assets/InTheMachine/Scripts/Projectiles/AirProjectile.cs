using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class AirProjectile : Projectile
{
    private float baseSpeed;
    private bool lifeOver;
    public override void ApplyProjectileProperties(Vector3 direction, float size, float speed, float lifetime, float power, LayerMask colliding, LayerMask piercing, LayerMask pinpoint)
    {
        base.ApplyProjectileProperties(direction, size, speed, lifetime, power, colliding, piercing, pinpoint);
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

    protected override void FixedUpdate()
    {
        if (lifeOver)
            return;
        _speed -= (baseSpeed / _lifetime) * Time.fixedDeltaTime;
        rb.velocity = Direction * Speed;
        base.FixedUpdate();
    }

    protected override void DoCollision(Collider2D collider)
    {
        bool doCollision = true;
        //if we hit a flammable object
        if (collider.TryGetComponent<IFlammable>(out IFlammable flame) || collider.transform.parent.TryGetComponent<IFlammable>(out flame))
        {
            //if it's on enemy layer
            if (collider.gameObject.layer == 7)
            {
                //make sure it's a real enemy and collide
                if (collider.attachedRigidbody && collider.attachedRigidbody.GetComponent<EnemyMachine>())
                {
                    flame.DouseFlame();
                }
                else doCollision = false;
            }
            else
            //if it's on the TakeFire layer, douse the flame and collide
            if (collider.gameObject.layer == 14)
            {
                if (flame.IsFlaming())
                {
                    flame.DouseFlame();
                }
                else
                    doCollision = false;
            }
        }
        if (doCollision)
            base.DoCollision(collider);
    }

    protected override void EndOfLife()
    {
        lifeOver = true;
        rb.simulated = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 2f);
    }

}
