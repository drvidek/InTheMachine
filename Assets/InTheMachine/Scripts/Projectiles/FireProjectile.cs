using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using UnityEngine.U2D;
public class FireProjectile : Projectile
{
    [SerializeField] private ParticleSystem _psysFire;
    private bool lifeOver;
    private float cost;

    public override void ApplyProjectileProperties(Vector3 direction, float size, float speed, float lifetime, float power, LayerMask colliding, LayerMask piercing)
    {
        cost = PlayerGun.main.Cost;
        transform.position = PlayerGun.main.SpawnPosition;
        if (Player.main.UserInputDir.y > 0.5f)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
            transform.localEulerAngles = new(0, 0, PlayerAnimate.main.FacingDirection.x > 0 ? 0 : 180);
        Player.main.onShootRelease += EndOfLife;
        Player.main.onFlyEnter += EndOfLife;
    }

    private void FixedUpdate()
    {
        if (lifeOver)
            return;

        rb.velocity = Vector2.zero;
        transform.position = PlayerGun.main.SpawnPosition;
        if (Player.main.UserInputDir.y > 0.5f)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
            transform.localEulerAngles = new(0, 0, PlayerAnimate.main.FacingDirection.x > 0 ? 0 : 180);

    }

    private void Update()
    {
        if (lifeOver)
            return;

        if (!Player.main.TryToUsePower(cost*Time.deltaTime))
            EndOfLife();
    }

    protected override void EndOfLife()
    {
        lifeOver = true;
        rb.simulated = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        _psysFire.Stop();
        Player.main.onShootRelease -= EndOfLife;
        Player.main.onFlyEnter -= EndOfLife;
        Destroy(gameObject, 1f);
    }
}
