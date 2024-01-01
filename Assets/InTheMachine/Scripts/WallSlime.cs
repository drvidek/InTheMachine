using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class WallSlime : MonoBehaviour, IFlammable, IElectrocutable, IProjectileTarget
{
    public void CatchFlame(Collider2D collider)
    {
        throw new System.NotImplementedException();
    }

    public void DouseFlame()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFlaming()
    {
        throw new System.NotImplementedException();
    }

    public void OnProjectileHit(Projectile projectile)
    {
        throw new System.NotImplementedException();
    }

    public void PropagateFlame(Collider2D collider)
    {
        throw new System.NotImplementedException();
    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {
        throw new System.NotImplementedException();
    }

    public void RecieveElectricity(Collider2D collider)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
