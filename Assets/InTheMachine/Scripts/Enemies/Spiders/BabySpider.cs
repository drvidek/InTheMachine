using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySpider : EnemyFlying, IFlammable
{
    [SerializeField] protected float turnSpeed;
    [SerializeField] protected float idleTime;
    [SerializeField] protected float avoidRange;

    public Vector2 Direction => currentDirection;

    Vector2 currentDirection, targetDirection, oldDirection;
    private float moveStartTime;

    protected bool ColliderAhead => Physics2D.Raycast(transform.position, _targetVelocity, avoidRange, groundedMask);

    protected override void OnIdleEnter()
    {
        Alarm alarm = Alarm.GetAndPlay(idleTime);
        alarm.onComplete = () =>
        {
            if (CurrentState == EnemyState.Idle)
                ChangeStateTo(EnemyState.Walk);
        };
        Halt();
        base.OnIdleEnter();
    }

    protected override void OnWalkEnter()
    {
        targetDirection.x = Random.Range(-1f, 1f);
        targetDirection.y = Random.Range(-1f, 1f);
        targetDirection.Normalize();
        oldDirection = currentDirection;
        moveStartTime = Time.time;

        Alarm alarm = Alarm.GetAndPlay(Random.Range(1f, 6f));
        alarm.onComplete = () =>
        {
            if (CurrentState == EnemyState.Walk)
            ChangeStateTo(EnemyState.Idle);
        };
    }

    protected override void OnWalkStay()
    {
        Move(currentDirection, moveSpeed);

        ChooseDirection();
    }

    protected void ChooseDirection()
    {
        currentDirection = Vector2.Lerp(oldDirection, targetDirection, (Time.time - moveStartTime) * turnSpeed);

        if (ColliderAhead || Random.Range(0f, 100f) > 80f)
        {
            targetDirection = Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward) * targetDirection;
            oldDirection = currentDirection;
        }
    }

    protected override void OnBurnEnter()
    {
        targetDirection.x = Random.Range(-1f, 1f);
        targetDirection.y = Random.Range(-1f, 1f);
        targetDirection.Normalize();
        oldDirection = currentDirection;
        moveStartTime = Time.time;
    }

    protected override void OnBurnStay()
    {
        Move(currentDirection, moveSpeed * 3f);

        PropagateFlame(_collider);

        ChooseDirection();

        TakeDamage(.5f * Time.fixedDeltaTime);

        base.OnBurnStay();
    }

    protected override void OnBurnExit()
    {
        DouseFlame();
        base.OnBurnExit();
    }

    protected override void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if (projectile is FireballProjectile or FireProjectile)
        {

        }
        TakeDamage(projectile.Power);
    }

    protected override void CheckForExternalVelocity()
    {

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

    public void CatchFlame(Collider2D collider)
    {
        EnemyCatchFlame(collider);
    }

    public void DouseFlame()
    {
        EnemyDouseFlame();
    }
    public bool IsFlaming()
    {
        return CurrentState == EnemyState.Burn;
    }

}
