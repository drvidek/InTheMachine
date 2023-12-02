using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public interface IElectrocutable
{
    public void RecieveElectricity();

    public static List<IElectrocutable> FindCircuit(Collider2D collider)
    {
        List<Collider2D> foundColliders = new();
        Collider2D currentColldier = collider;
        foundColliders.Add(currentColldier);
        bool newFound = true;

        int i = 0;
        while (newFound || i < foundColliders.Count)
        {
            currentColldier = foundColliders[i];
            List<Collider2D> currentNeighbours = FindImmediateNeighbours(currentColldier);
            newFound = false;

            foreach (Collider2D node in currentNeighbours)
            {
                if (foundColliders.Contains(node))
                    continue;

                newFound = true;
                foundColliders.Add(node);
            }
            i++;
        }

        List<IElectrocutable> circuit = new();
        foreach (var item in foundColliders)
        {
            circuit.Add(item.GetComponent<IElectrocutable>());
        }

        return circuit;
    }

    public static List<Collider2D> FindImmediateNeighbours(Collider2D collider)
    {
        List<Collider2D> neighbours = new();

        Collider2D[] colliders = new Collider2D[10];
        ContactFilter2D filter = new();
        Physics2D.OverlapCollider(collider, filter.NoFilter(), colliders);
        foreach (var item in colliders)
        {
            if ((item != null) && (item.GetComponent<IElectrocutable>() != null) && !neighbours.Contains(item))
                neighbours.Add(item);
        }

        return neighbours;
    }

}
