using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlammable
{
    protected virtual List<IFlammable> FindNeighbours()
    {
        List<IFlammable> neighbours = new();


        return neighbours;
    }

    protected virtual void PropagateFlame()
    {

    }
}
