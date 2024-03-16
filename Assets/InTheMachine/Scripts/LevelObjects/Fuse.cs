using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Fuse : MonoBehaviour, IFlammable
{
    [SerializeField] private float burnTime;
    [SerializeField] private bool burnWithSmoke;
    [SerializeField] private Collider2D _collider;
    private bool burning;
    private bool canBurn = true;
    private GameObject burnEffect;
    private Color baseColor;

    public void CatchFlame(Collider2D collider)
    {
        if (burning || !canBurn)
            return;
        burning = true;
        Alarm propagate = AlarmPool.GetAndPlay(burnTime);
        propagate.onComplete = () => { PropagateFlame(_collider); DouseFlame(); };
        burnEffect = Instantiate(IFlammable.psysObjFire, transform);
        if (burnWithSmoke)
            Instantiate(IFlammable.psysObjSmoke, burnEffect.transform);
    }

    public void DouseFlame()
    {
        burning = false;
        canBurn = false;

        Alarm readyToBurn = AlarmPool.GetAndPlay(burnTime * 10f);
        readyToBurn.onComplete = () => canBurn = true;
        if (burnEffect)
        {
            IFlammable.ClearFireAndSmoke(burnEffect);
            Destroy(burnEffect,burnTime * 2f);
        }
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
        throw new System.NotImplementedException();
    }
}
