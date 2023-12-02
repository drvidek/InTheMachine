using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobweb : Debris, IFlammable
{
    [SerializeField] private float timeToLight;
    [SerializeField] private float burnTime;
    [SerializeField] private ParticleSystem flames;
    private float flameCurrent;
    private bool catching;
    private bool burning;
    private bool burntOut;
    private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponentInChildren<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!burning && !catching)
        {
            //DouseFlame();
        }

        if (flameCurrent > timeToLight)
        {
            if (!burning)
            {
                flames.Play();
                burning = true;
            }
        }

        if (burning && !burntOut)
        {
            PropagateFlame(transform.position, Vector2.one);
            flameCurrent += Time.fixedDeltaTime;
            if (flameCurrent > burnTime + timeToLight)
            {
                burntOut = true;
                OnProjectileHit(null);
                flames.Stop();
            }
        }

        catching = false;
    }

    public void CatchFlame(Collider2D flameSource)
    {
        catching = true;
        flameCurrent += Time.fixedDeltaTime;
    }

    public void DouseFlame()
    {
        flameCurrent = 0;
    }

    public void PropagateFlame(Collider2D collider)
    {

    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(position, size))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame(_collider);
        }
    }

    public bool IsFlaming()
    {
        return burning;
    }
}
