using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour, IFlammable
{
    [SerializeField] private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (_collider.enabled)
            PropagateFlame(_collider);
    }
    public void CatchFlame(Collider2D collider)
    {
        
    }

    public void DouseFlame()
    {
      
    }

    public bool IsFlaming()
    {
        return _collider.enabled;
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
}
