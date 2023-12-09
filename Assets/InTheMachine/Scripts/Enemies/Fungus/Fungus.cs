using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fungus : EnemyStatic, IFlammable, IProjectileTarget
{
    [SerializeField] protected float catchFlameTime;
    private Meter catchFlame = new(0, 1, 0);

    protected override void Start()
    {
        catchFlame.onMax += () => EnemyCatchFlame(null);
        base.Start();
    }

    protected override void OnBurnStay()
    {
        PropagateFlame(_collider);
        TakeDamage(5f * Time.fixedDeltaTime);
        base.OnBurnStay();
    }

    protected override void OnBurnExit()
    {
        DouseFlame();
        base.OnBurnExit();
    }
    public void CatchFlame(Collider2D collider)
    {
        catchFlame.FillOver(catchFlameTime, true, true, true);
        //EnemyCatchFlame(collider);
    }

    public void DouseFlame()
    {
        EnemyDouseFlame();
    }

    public bool IsFlaming()
    {
        return burning;
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

    }

    protected override void OnDieEnter()
    {
        base.OnDieEnter();
    }

    public void OnProjectileHit(Projectile projectile)
    {
        
        
        
        
    }
}
