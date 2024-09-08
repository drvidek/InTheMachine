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
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Launcher fireballLauncher;
    [SerializeField] private Collider2D burstCollider;
    [SerializeField] private CapsuleCollider2D flamethrowerCollider;
    [SerializeField] private GameObject reward;
    [SerializeField] private UnityEvent killEvent;

    public Action onFlamethrower, onBurst, onCharge, onFireball;

    private bool vulnerable;

    private int fireballCount, fireballMax;

    private Attack currentAttack;
    public Attack CurrentAttack => currentAttack;

    private Attack[] previousAttack = new Attack[2];

    private float maxLength = 4f;

    private float WalkSpeed => _walkSpeed * (SecondPhase ? 2f : 1f);
    private float ChargeAttackSpeed => chargeAttackSpeed * (SecondPhase ? 1.5f : 1f);

    public bool SecondPhase => healthMeter.Percent <= 0.5f;

    public bool GroundAhead => Physics2D.Raycast(transform.position, new Vector2(CurrentDirection.x, -1), 1f, groundedMask);

    public override Collider2D ColliderAhead
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _collider.bounds.size, transform.localEulerAngles.z, CurrentDirection, 0.2f, groundedMask);
            if (!hit)
                return null;
            return hit.collider;
        }
    }

    protected override void Start()
    {
        alarmBook.AddAlarm("BurstCollider", 0.8f, false).onComplete = () => burstCollider.enabled = false;
        var idleAlarm = alarmBook.AddAlarm("Idle", 0.5f, false);
        var attackAlarm = alarmBook.AddAlarm("Attack", burstAttackRecovery, false);
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
        fireballLauncher = GetComponent<Launcher>();
        base.Start();
    }

    protected override void CheckPlayerInRangeForLogic(Vector3Int room)
    {
        if (doNotPersist)
        {
            doingLogic = CameraController.main.SpecialCamActive;
            if (!doingLogic)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if (everDoneLogic)
                doingLogic = RoomManager.main.PlayerWithinRoomDistance(transform);
            else
                doingLogic = RoomManager.main.PlayerWithinRoomDistance(transform, 0);
        }

        everDoneLogic = doingLogic || everDoneLogic;

        if (IsAlive)
        {
            rb.simulated = doingLogic;
            if (agentAnimator)
                agentAnimator.SetEnabled(doingLogic);
        }
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
        if (!playerInRoom && !doNotPersist)
            healthMeter.Fill();
    }

    protected override void OnIdleStay()
    {
        if (!playerInRoom && !doNotPersist)
            alarmBook.GetAlarm("Idle").ResetAndPlay();

        MoveWithGravity(Vector3.zero, 0f);
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


        MoveWithGravity(CurrentDirection, WalkSpeed);
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
                alarm.ResetAndPlay(burstAttackDelay).onComplete = () =>
                {
                    if (!IsAlive)
                        return;
                    onBurst?.Invoke();
                    burstCollider.enabled = true;
                    alarmBook.GetAlarm("BurstCollider").ResetAndPlay();
                    alarm.ResetAndPlay(burstAttackRecovery).onComplete =
                        ChooseNewAttack;
                };
                break;
            case Attack.Fireball:
                onFireball?.Invoke();
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
                MoveWithGravity(CurrentDirection, WalkSpeed);
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
                    MoveWithGravity(Vector3.zero, 0f);
                }
                break;
            case Attack.Fireball:
                {
                    walkingRight = Player.main.X > transform.position.x;
                    _targetVelocity = Vector2.zero;
                    MoveWithGravity(Vector3.zero, 0f);
                }
                break;
            case Attack.Charge:
                MoveWithGravity(CurrentDirection, ChargeAttackSpeed);
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
        //if we used the same attack twice in a row, prevent it from being used again
        if (previousAttack[0] == previousAttack[1] && previousAttack[1] == currentAttack)
        {
            ChooseNewAttack();
            return;
        }
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
                walkingRight = Player.main.X > transform.position.x;
                fireballLauncher.TryToShoot();
                fireballCount = 0;
                fireballMax = UnityEngine.Random.Range(2, 4);
                var alarm = alarmBook.GetAlarm("Attack");
                alarm.ResetAndPlay(SecondPhase ? 0.5f : 1f).onComplete = () =>
                {
                    fireballLauncher.TryToShoot();
                    fireballCount++;
                    alarm.ResetAndPlay();
                    if (fireballCount == fireballMax)
                    {
                        alarm.onComplete = ChooseNewAttack;
                    }
                };
                ChangeStateTo(EnemyState.Attack);
                break;
            default:
                break;
        }
        var attackTemp = previousAttack[0];
        previousAttack[0] = currentAttack;
        previousAttack[1] = attackTemp;
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
