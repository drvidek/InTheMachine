using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using UnityEngine.U2D;
public class FireProjectile : Projectile, IFlammable
{
    [SerializeField] private ParticleSystem _psysFire;
    [SerializeField] private LayerMask blockingLayer;
    private bool lifeOver;
    private float cost;
    private CapsuleCollider2D _collider;

    private float maxLength = 4.5f;

    public override void ApplyProjectileProperties(Vector3 direction, float size, float speed, float lifetime, float power, LayerMask colliding, LayerMask piercing)
    {
        cost = PlayerGun.main.Cost;
        transform.position = PlayerGun.main.SpawnPosition;
        if (Player.main.UserInputDir.y > 0.5f)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
            transform.localEulerAngles = new(0, 0, PlayerAnimate.main.FacingDirection.x > 0 ? 0 : 180);
        Player.main.onShootRelease += EndOfLife;
        Player.main.onFlyEnter += EndOfLife;
        _collider = GetComponentInChildren<CapsuleCollider2D>();
        UpdateCollider(Vector2.one);
    }

    private void FixedUpdate()
    {
        PropagateFlame(_collider);
        rb.velocity = Vector2.zero;


        if (lifeOver)
            return;

        float newX = Mathf.MoveTowards(_collider.size.x, maxLength, maxLength * Time.fixedDeltaTime);
        UpdateCollider(new(newX, 1));

        transform.position = PlayerGun.main.SpawnPosition;

        if (Player.main.UserInputDir.y > 0.5f)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
            transform.localEulerAngles = new(0, 0, PlayerAnimate.main.FacingDirection.x > 0 ? 0 : 180);

    }

    private void Update()
    {
        if (lifeOver)
            return;

        if (!Player.main.TryToUsePower(cost * Time.deltaTime))
            EndOfLife();
    }

    protected void UpdateCollider(Vector2 size)
    {
        _collider.size = size;
        _collider.offset = new(size.x / 2f, 0);
    }


    protected override void EndOfLife()
    {
        lifeOver = true;
        rb.simulated = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        _psysFire.Stop();

        Player.main.onShootRelease -= EndOfLife;
        Player.main.onFlyEnter -= EndOfLife;
        Destroy(gameObject, 1f);
    }

    public void PropagateFlame(Collider2D collider)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        int i = 0;
        Vector3 start = transform.position;
        foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
        {
            Vector3 end = (flammable as MonoBehaviour).GetComponent<Collider2D>().bounds.center;
            if (flammable != thisFlam &&
                !Physics2D.Raycast(start, QMath.Direction(start, end), Vector3.Distance(start, end), blockingLayer))
                flammable.CatchFlame(collider);
            i++;
        }
    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public void CatchFlame(Collider2D collider)
    {

    }

    public void DouseFlame()
    {

    }

    public bool IsFlaming()
    {
        return true;
    }
}
