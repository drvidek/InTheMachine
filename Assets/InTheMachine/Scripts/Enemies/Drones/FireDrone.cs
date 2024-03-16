using QKit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireDrone : EnemyWalking
{
    public enum Attack
    {
        Null,
        Flamethrower,
        Burst,
        Fireball,
        Charge
    }

    [SerializeField] private float chargeAttackSpeed, burstAttackDelay, burstAttackRecovery;

    [SerializeField] private Collider2D burstCollider;
    [SerializeField] private CapsuleCollider2D flamethrowerCollider;
    [SerializeField] private GameObject reward;
    [SerializeField] private UnityEvent killEvent;

    public Action onFlamethrower, onBurst, onCharge;

    private bool vulnerable;

    private Attack currentAttack;
    public Attack CurrentAttack => currentAttack;

    private float maxLength = 4f;

    public bool GroundAhead => Physics2D.Raycast(transform.position, new Vector2(CurrentDirection.x, -1), 1f, groundedMask);

    protected override void Start()
    {
        var idleAlarm = alarmBook.AddAlarm("Idle",0.5f, false);
        var attackAlarm = alarmBook.AddAlarm("Attack",burstAttackRecovery, false);
        idleAlarm.onComplete = () =>
        {
            if (!GroundAhead || ColliderAhead)
                ChangeDirection();
            ChooseNewAttack();
        };

        onFlamethrower += () =>
        {
            flamethrowerCollider.enabled = true;
            flamethrowerCollider.size = new(0, 0.5f);
        };
        base.Start();
    }


    protected virtual void SetVulnerable(float isVulnerable)
    {
        vulnerable = isVulnerable == 0 ? false : true;
    }


    protected override void OnIdleEnter()
    {
        _targetVelocity = Vector2.zero;
        rb.velocity = _targetVelocity;
        alarmBook.GetAlarm("Idle").ResetAndPlay();
        if (!playerInRoom)
            healthMeter.Fill();
    }

    protected override void OnIdleStay()
    {
        if (!playerInRoom)
            alarmBook.GetAlarm("Idle").ResetAndPlay();
    }

    protected override void OnWalkEnter()
    {
        if (CurrentAttack == Attack.Flamethrower)
            ChangeStateTo(EnemyState.Attack);
    }

    protected override void OnWalkStay()
    {
        if (currentAttack == Attack.Burst && playerInRange)
        {
            ChangeStateTo(EnemyState.Attack);
        }

        if (!GroundAhead || ColliderAhead)
        {
            ChangeDirection();
            ChooseNewAttack();
        }


        MoveWithGravity(CurrentDirection, _walkSpeed);
    }

    protected override void OnAttackEnter()
    {
        switch (currentAttack)
        {
            case Attack.Flamethrower:
                onFlamethrower?.Invoke();

                break;
            case Attack.Charge:
                onCharge?.Invoke();
                break;
            case Attack.Burst:
                onPreAttack?.Invoke();
                var alarm = alarmBook.GetAlarm("Attack");
                alarm.ResetAndPlay(burstAttackDelay);
                alarm.onComplete = () =>
                {
                    onBurst?.Invoke();
                    burstCollider.enabled = true;
                    Alarm killCollider = AlarmPool.GetAndPlay(0.8f);
                    killCollider.onComplete = () => burstCollider.enabled = false;
                    alarm.ResetAndPlay(burstAttackRecovery);
                    alarm.onComplete = () =>
                        ChooseNewAttack();
                };
                break;
            case Attack.Fireball:
                //attackAlarm.onComplete = () =>
                //{
                //
                //};
                break;
            default:
                break;
        }
    }

    protected override void OnAttackStay()
    {
        switch (currentAttack)
        {
            case Attack.Flamethrower:
                MoveWithGravity(CurrentDirection, _walkSpeed);
                float newX = Mathf.MoveTowards(flamethrowerCollider.size.x, maxLength, maxLength * Time.fixedDeltaTime);
                UpdateFlamethrowerCollider(new(newX, 0.5f));
                if (!GroundAhead || ColliderAhead)
                {
                    ExitAttack();
                }
                break;
            case Attack.Burst:
                {
                    _targetVelocity = Vector2.zero;
                }
                break;
            case Attack.Fireball:
                //ChooseNewAttack();
                break;
            case Attack.Charge:
                MoveWithGravity(CurrentDirection, chargeAttackSpeed);
                if (!GroundAhead || ColliderAhead)
                {
                    ExitAttack();
                }
                break;
            default:
                break;
        }
    }

    protected override void OnAttackExit()
    {
        flamethrowerCollider.enabled = false;
        burstCollider.enabled = false;
        base.OnAttackExit();
    }

    protected override void OnDieEnter()
    {
        Instantiate(reward, transform.position, Quaternion.identity);
        killEvent?.Invoke();
        base.OnDieEnter();
    }

    private void ExitAttack()
    {
        currentAttack = Attack.Null;
        ChangeStateTo(EnemyState.Idle);
    }

    private void ChangeDirection()
    {
        walkingRight = !walkingRight;
    }

    private void ChooseNewAttack()
    {
        currentAttack = QMath.Choose<Attack>(Attack.Burst, Attack.Charge, Attack.Fireball, Attack.Flamethrower);
        switch (currentAttack)
        {
            case Attack.Flamethrower:
                if (IsAttacking)
                    onFlamethrower?.Invoke();
                ChangeStateTo(EnemyState.Attack);

                break;
            case Attack.Charge:
                ChangeStateTo(EnemyState.Attack);
                break;
            case Attack.Burst:
                ChangeStateTo(EnemyState.Walk);
                break;
            case Attack.Fireball:
                ChooseNewAttack();
                //ChangeStateTo(EnemyState.Idle);
                break;
            default:
                break;
        }
    }

    protected override void Move(Vector3 direction, float speed)
    {

    }

    public override Rigidbody2D GetStandingOnRigidbody()
    {
        return null;
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if (!vulnerable)
            return;

        TakeDamage(projectile.Power);

    }

    protected void UpdateFlamethrowerCollider(Vector2 size)
    {
        flamethrowerCollider.size = size;
        flamethrowerCollider.offset = new(size.x / 2f * CurrentDirection.x, 0);
    }

    protected override void CheckForExternalVelocity()
    {

    }
}
