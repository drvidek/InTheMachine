using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FireballProjectile : Projectile, IFlammable
{
    private Collider2D _collider;

    protected override void Start()
    {
        _collider = GetComponentInChildren<Collider2D>();
        base.Start();
    }

    protected override void FixedUpdate()
    {
        PropagateFlame(_collider);
        base.FixedUpdate();
    }

    public void CatchFlame(Collider2D collider)
    {
        
    }

    public void DouseFlame()
    {
        
    }

    public bool IsFlaming()
    {
        return true;
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
        throw new System.NotImplementedException();
    }
}
