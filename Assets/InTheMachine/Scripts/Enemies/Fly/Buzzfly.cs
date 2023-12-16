using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Buzzfly : Fly
{
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackDelayTime;

    private Alarm attackDelay;
    private float currentSpeed;

    private Vector3 direction;

    private float currentDistanceToPlayer => Vector3.Distance(transform.position, Player.main.Position);
    

    protected override void Start()
    {
        attackDelay = Alarm.Get(attackDelayTime, false, false);
        attackDelay.onComplete += () => { direction = QMath.Direction(transform.position, Player.main.Position); currentSpeed = attackSpeed; };
        base.Start();
    }

    protected override void OnFlyStay()
    {
        TryToAttack();
        base.OnFlyStay();
    }

    protected override void OnIdleStay()
    {
        TryToAttack();
        base.OnIdleStay();
    }

    protected override void OnAttackEnter()
    {
        attackDelay.ResetAndPlay(attackDelayTime + Random.Range(-0.1f,0.1f));
        _targetVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
        base.OnAttackEnter();
    }

    protected override void OnAttackStay()
    {
        if (attackDelay.IsStopped)
        {
            Move(direction, currentSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, attackSpeed * Time.fixedDeltaTime);
            if (currentSpeed == 0)
            {
                attackDelay.ResetAndPlay();
                _targetVelocity = Vector2.zero;
                rb.velocity = Vector2.zero;
            }
        }
        EnemyState nextState =
            !CheckAttackCondition() ? EnemyState.Idle :
            _currentState;
        ChangeStateTo(nextState);
    }

    protected override void OnAttackExit()
    {
        attackDelay.Stop();
        base.OnAttackExit();
    }

    protected override bool CheckAttackCondition()
    {
        return playerInRange && playerInRoom && playerInSight;
    }

}
