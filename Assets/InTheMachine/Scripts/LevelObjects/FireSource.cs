using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
public class FireSource : MonoBehaviour, IActivate, IFlammable, IProjectileTarget
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ParticleSystem psysFlame, psysGas;
    [SerializeField] private bool isLit;
    private bool isLocked;

    private void Start()
    {
        if (isLit)
            CatchFlame();
        else
            DouseFlame();
    }

    private void FixedUpdate()
    {
        if (isLit)
            PropagateFlame(_collider);
    }

    public void CatchFlame()
    {
        if (isLocked)
            return;
        isLit = true;
        psysFlame.Play();
        psysGas.Stop();
    }

    public void DouseFlame()
    {
        if (isLocked)
            return;
        isLit = false;
        psysGas.Play();
        psysFlame.Stop();
        psysFlame.Clear();
    }

    public void OnProjectileHit(Projectile projectile)
    {
        switch (projectile.Power)
        {
            case (float)GunProfileType.Air:
                DouseFlame();
                break;
            case (float)GunProfileType.Fire:
                CatchFlame();
                break;
            default:
                break;
        }
    }

    public void PropagateFlame(Collider2D collider)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame();
        }
    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public void ToggleActive(bool active)
    {
        CatchFlame();
    }

    public void ToggleActiveAndLock(bool active)
    {
        CatchFlame();
        isLocked = true;
    }

}
