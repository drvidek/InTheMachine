using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusJump : Fungus
{
    [SerializeField] private float jumpPower, descendSpeed, homeTolerance;

    private EnemyMachine stem;

    private Vector3 homePosition;

    private float currentSpeed;



    protected override void Start()
    {
        homePosition = transform.position;
        foreach (var item in Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0, 1 << 7))
        {
            if (item.TryGetComponent<FungusStem>(out FungusStem e))
            {
                stem = e;
                break;
            }
        }
        base.Start();
    }

    protected override void OnIdleEnter()
    {
        _targetVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    protected override void OnIdleStay()
    {
        rb.velocity = Vector3.zero;
        transform.position = homePosition;
        if (CheckAttackCondition())
            ChangeStateTo(EnemyState.Ascend);

    }

    protected override void OnAscendEnter()
    {
        currentSpeed = jumpPower;
    }

    protected override void OnAscendStay()
    {
        Move(Vector3.up, currentSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0, _fric * Time.fixedDeltaTime);

        if (currentSpeed == 0)
        {
            ChangeStateTo(EnemyState.Descend);
        }
    }

    protected override void OnDescendStay()
    {
        Move(Vector3.down, descendSpeed);

        if (Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.down, 0.02f, groundedMask))
        {
            ChangeStateTo(EnemyState.Stun);
        }

        if (stem.CurrentState != EnemyState.Die)
        {
            if (Vector2.Distance(transform.position, stem.transform.position) < homeTolerance)
            {
                transform.position = homePosition;
                ChangeStateTo(EnemyState.Idle);
            }

        }

    }

    protected override void OnBurnEnter()
    {
        currentSpeed = rb.velocity.y;
        base.OnBurnEnter();
    }

    protected override void OnBurnStay()
    {
        currentSpeed -= _fric * Time.fixedDeltaTime;
        if (Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.down, 0.02f, groundedMask))
        {
            currentSpeed = 0;
        }
        Move(new(0, currentSpeed), 1);
        base.OnBurnStay();
    }

    protected override void OnStunEnter()
    {
        _targetVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    protected override void OnStunStay()
    {
        rb.velocity = Vector3.zero;

        if (!Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.down, 0.02f, groundedMask))
            ChangeStateTo(EnemyState.Descend);
    }

    protected override void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    public override void OnProjectileHit(Projectile projectile)
    {

    }

    protected override void CheckForExternalVelocity()
    {

    }

    protected override bool CheckAttackCondition()
    {
        return playerInRange && playerInRoom;
    }
}
