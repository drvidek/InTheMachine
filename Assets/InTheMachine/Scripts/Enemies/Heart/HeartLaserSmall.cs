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

    protected override void CheckPlayerInRangeForLogic(Vector3Int room)
    {
        //force the laser on no matter what room the player is in
        doingLogic = true;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        
    }
}
