using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public interface IFlammable
{
    public static GameObject psysObjFire = Resources.Load("PSysFire") as GameObject;
    public static GameObject psysObjSmoke = Resources.Load("PSysSmoke") as GameObject;
    public static GameObject psysObjSmokePuff = Resources.Load("PSysSmokePuff") as GameObject;


    public static GameObject InstantiateFireAndSmoke(Transform transform)
    {
        GameObject go = MonoBehaviour.Instantiate(psysObjFire, transform);
        MonoBehaviour.Instantiate(psysObjSmoke, go.transform);
        return go;
    }

    public static GameObject InstantiateSmokePuff(Transform transform)
    {
        GameObject go = MonoBehaviour.Instantiate(psysObjSmokePuff, transform.position,Quaternion.identity);
        return go;
    }

    public static void ClearFireAndSmoke(GameObject obj)
    {
        obj.GetComponentInChildren<ParticleSystem>().Stop();
        GameObject.Destroy(obj, 5f);
    }

    public static List<IFlammable> FindFlammableNeighbours(Collider2D collider)
    {
        List<IFlammable> neighbours = new();

        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D filter = new();
        Physics2D.OverlapCollider(collider, filter.NoFilter(), colliders);
        IFlammable f = null;
        foreach (var item in colliders)
        {
            if (item != null &&
                (item.TryGetComponent<IFlammable>(out f) || (item.gameObject.layer == 14 && item.transform.parent.TryGetComponent<IFlammable>(out f))) &&
                !neighbours.Contains(f))
            {

                neighbours.Add(f);
            }
        }

        return neighbours;
    }

    public static List<IFlammable> FindFlammableNeighbours(Vector3 position, Vector2 size)
    {
        List<IFlammable> neighbours = new();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, size, 0);
        IFlammable f = null;

        foreach (var item in colliders)
        {
            if (item != null &&
                (item.TryGetComponent<IFlammable>(out f) || (item.gameObject.layer == 14 && item.transform.parent.TryGetComponent<IFlammable>(out f))) &&
                !neighbours.Contains(f))
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
    public void CatchFlame(Collider2D collider);
    public void DouseFlame();

    public bool IsFlaming();
}
