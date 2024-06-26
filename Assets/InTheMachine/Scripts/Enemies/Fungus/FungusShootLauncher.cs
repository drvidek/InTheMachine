using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FungusShootLauncher : Launcher
{
    protected override bool CanShoot()
    {
        return true;
    }

    protected override Vector3 GetDirection()
    {
        return transform.up;
    }

    protected override void Shoot()
    {
        Vector3 direction = GetDirection();
        if (!_projectilePrefab)
        {
            Debug.LogError($"No projectile prefab is installed in {gameObject.name}'s launcher.");
            return;
        }
        float halfSpread = -_spread / 2f;
        Vector3 currentDirection = Quaternion.AngleAxis(halfSpread,Vector3.forward) * direction;
        for (int i = 0; i < _count; i++)
        {
            Vector3 newDirection = Quaternion.AngleAxis(i * (_spread/(_count-1)), Vector3.forward) * currentDirection;
            Projectile projectile = Instantiate(_projectilePrefab, _spawn.position, Quaternion.identity, null);
            ApplyPropertiesToProjectile(projectile, newDirection);

        }

    }

    protected override void Reload()
    {

    }
}
