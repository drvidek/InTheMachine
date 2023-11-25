using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlammable
{
    public static List<IFlammable> FindFlammableNeighbours(Collider2D collider)
    {
        List<IFlammable> neighbours = new();

        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D filter = new();
        Physics2D.OverlapCollider(collider, filter.NoFilter(), colliders);
        foreach (var item in colliders)
        {
            if (item != null && item.TryGetComponent<IFlammable>(out IFlammable f) && !neighbours.Contains(f))
                neighbours.Add(f);
        }

        return neighbours;
    }

    public static List<IFlammable> FindFlammableNeighbours(Vector3 position, Vector2 size)
    {
        List<IFlammable> neighbours = new();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, size, 0);
        foreach (var item in colliders)
        {
            if (item != null && item.TryGetComponent<IFlammable>(out IFlammable f) && !neighbours.Contains(f))
                neighbours.Add(f);
        }

        return neighbours;
    }

    public void PropagateFlame(Collider2D collider);
    //{
    //    IFlammable thisFlam = GetComponentInChildren<IFlammable>();
    //    foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
    //    {
    //        if (flammable != thisFlam)
    //            flammable.CatchFlame();
    //    }
    //}

    public void PropagateFlame(Vector3 position, Vector2 size);
    //{
    //    IFlammable thisFlam = GetComponentInChildren<IFlammable>();
    //    foreach (var flammable in IFlammable.FindFlammableNeighbours(position, size))
    //    {
    //        if (flammable != thisFlam)
    //            flammable.CatchFlame();
    //    }
    //}
    public void CatchFlame();
    public void DouseFlame();
}
