using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyWalking
{
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpDelayTime;
    private Alarm jumpAlarm;

    protected override void Start()
    {
        jumpAlarm = alarmBook.AddAlarm("Jump",jumpDelayTime, false);
        jumpAlarm.onComplete = () => ChangeStateTo(EnemyState.Ascend);
        base.Start();
    }

    protected override void OnIdleEnter()
    {
        jumpAlarm.ResetAndPlay(jumpDelayTime + Random.Range(-0.2f, 0.5f));
        base.OnIdleEnter();
    }

    protected override void OnIdleStay()
    {
        MoveWithGravity(Vector2.zero, 0);
        base.OnIdleStay();
    }

    protected override void OnAscendEnter()
    {
        ChooseDirection();

        _targetVelocity.x = _walkSpeed * (walkingRight ? 1 : -1) + Random.Range(-_walkSpeed / 4f, _walkSpeed / 4f);
        _targetVelocity.y = jumpPower * Random.Range(0.8f, 1.2f);
    }

    protected override void OnAscendStay()
    {
        MoveWithGravity(CurrentDirection, _walkSpeed);

        if (IsGrounded && rb.velocity.y <= 0)
            ChangeStateTo(EnemyState.Idle);
    }

    protected virtual void ChooseDirection()
    {
        walkingRight = QMath.Choose<bool>(true, false);

        if (Physics2D.Raycast(transform.position, walkingRight ? Vector2.right : Vector2.left, 1f, groundedMask))
            walkingRight = !walkingRight;
    }

    public override Rigidbody2D GetStandingOnRigidbody()
    {
        if (!StandingOn)
            return null;
        return StandingOn.GetComponent<Rigidbody2D>();
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile)
            return;
        TakeDamage(projectile.Power);
    }

    protected override void CheckForExternalVelocity()
    {
        Rigidbody2D rb = GetStandingOnRigidbody();
        if (!rb)
            return;
        if (rb.velocity.magnitude == 0)
            return;
        if (externalVelocitySources.ContainsKey(rb))
            return;

        externalVelocitySources.Add(rb, rb.position);
    }
}
