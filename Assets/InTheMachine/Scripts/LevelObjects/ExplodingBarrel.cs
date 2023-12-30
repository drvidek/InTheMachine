using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using QKit;

public class ExplodingBarrel : MonoBehaviour, IFlammable, IProjectileTarget
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private bool respawn;
    private Rigidbody2D rb;
    private Vector3 homePos;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        homePos = transform.position;
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(explosion, 0.75f);

        PropagateFlame(explosion.GetComponent<Collider2D>());
        if (respawn)
            transform.position = homePos;
        else
            Destroy(gameObject);
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (rb.velocity.magnitude > )
    //         Explode();
    // }

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

    public void CatchFlame(Collider2D collider)
    {
        Explode();
    }

    public void DouseFlame()
    {

    }

    public bool IsFlaming()
    {
        return false;
    }

    public void OnProjectileHit(Projectile projectile)
    {

        Explode();

    }
}
