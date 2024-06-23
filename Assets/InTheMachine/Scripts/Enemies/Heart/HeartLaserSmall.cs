using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLaserSmall : EnemyStatic
{
    [SerializeField] private float speed = 180;
    float sign = 1;

    private void OnEnable()
    {
        sign = QMath.Choose<float>(1f, -1f);
    }

    protected override void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, sign * speed * Time.fixedDeltaTime);
        base.FixedUpdate();
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        
    }
}
