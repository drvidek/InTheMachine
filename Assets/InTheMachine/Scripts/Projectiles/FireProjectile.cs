using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FireProjectile : Projectile, IFlammable
{
    [SerializeField] private LayerMask blockingLayer;
    [SerializeField] private AnimationCurve curve;

    private Alarm endOfLife;
    private Collider2D _collider;
    private float startTime;

    protected override void Start()
    {
        startTime = Time.time;
        _collider = GetComponentInChildren<Collider2D>();
        endOfLife = Alarm.GetAndPlay(_lifetime);
        endOfLife.onComplete = EndOfLife;
        if (Direction.x != 0)
        _speed += Mathf.Abs(Player.main.rb.velocity.x);
        base.Start();
    }

    private void FixedUpdate()
    {
        float speed = Speed * curve.Evaluate(Time.time - startTime);
        Move(Direction, speed);
        PropagateFlame(_collider);
    }

    public void PropagateFlame(Collider2D collider)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        int i = 0;
        foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
        {
            if (flammable != thisFlam)
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

    protected override void DoCollision(Collider2D collider)
    {
        if (endOfLife != null)
        {
            endOfLife.Stop();
            endOfLife.onComplete -= EndOfLife;

        }
        base.DoCollision(collider);
    }

    public bool IsFlaming()
    {
        return true;
    }
}
