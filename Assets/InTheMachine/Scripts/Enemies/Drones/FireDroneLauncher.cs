using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FireDroneLauncher : Launcher
{
    protected override bool CanShoot()
    {
        return true;
    }

    protected override Vector3 GetDirection()
    {
        return QMath.Direction(transform.position, Player.main.Position);
    }

    protected override void Reload()
    {

    }
    protected override void ApplyPropertiesToProjectile(Projectile projectile, Vector3 direction)
    {
        base.ApplyPropertiesToProjectile(projectile, direction);
        projectile.gameObject.layer = 7;
        projectile.transform.GetChild(0).gameObject.layer = 7;
    }
}
