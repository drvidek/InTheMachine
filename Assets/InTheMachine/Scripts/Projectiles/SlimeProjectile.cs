using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class SlimeProjectile : Projectile
{
    [SerializeField] float grav = 15f;
    [SerializeField] float gravDelay = 0.5f;
    [SerializeField] float fric = 15f;
    [SerializeField] private ParticleSystem psysTrail, psysSplash;
    private bool dead;
    private float currentGrav;

    protected override void Start()
    {
        Alarm gravAlarm = Alarm.GetAndPlay(gravDelay);
        gravAlarm.onComplete = () => currentGrav = grav;
        if (_direction.y != 0)
            _direction.x = 0;
        _direction = _direction * (_speed + Random.Range(-2f, 2f));
        base.Start();
    }


    private void FixedUpdate()
    {
        if (dead)
            return;
        _direction.y -= currentGrav * Time.fixedDeltaTime;
        _direction.x = Mathf.MoveTowards(_direction.x, 0, fric * Time.fixedDeltaTime);
        rb.velocity = Direction;
    }

    protected override void EndOfLife()
    {
        dead = true;
        psysSplash.Play();
        psysTrail.Stop();
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        rb.simulated = false;
        Destroy(gameObject, 1f);
    }
}
