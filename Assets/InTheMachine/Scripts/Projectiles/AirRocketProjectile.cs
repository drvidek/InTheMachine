using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
public class AirRocketProjectile : Projectile
{
    [SerializeField] private float accelPerSec;
    [SerializeField] private ParticleSystem psysTrail, psysBoom;

    protected override void Start()
    {
        transform.localEulerAngles = new(0, 0, Direction.y > 0 ? 90 : Direction.x < 0 ? 180 : 0);
        base.Start();
    }

    private void FixedUpdate()
    {
        _speed += accelPerSec * Time.fixedDeltaTime;
        rb.velocity = Direction * Speed;
    }

    protected override void EndOfLife()
    {
        psysBoom.Play();
        psysTrail.Stop();
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        rb.simulated = false;
        Destroy(gameObject, 1f);
    }
}
