using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Spider : Beetle
{
    [SerializeField] private float actionDelay = 0.5f;
    [SerializeField] private float jumpVerticalPower, jumpHorizontalPower;

    private Alarm nextActionAlarm;


    protected override void Start()
    {
        nextActionAlarm = Alarm.Get(actionDelay, false, false);
        nextActionAlarm.onComplete = DetermineAction;
        base.Start();
    }

    protected override void OnIdleEnter()
    {
        nextActionAlarm.ResetAndPlay(actionDelay + Random.Range(-0.5f, 0.5f));
        Halt();
    }

    protected override void OnIdleStay()
    {
        //override beetle behaviour
    }

    protected override void OnWalkEnter()
    {
        nextActionAlarm.ResetAndPlay(3f + Random.Range(-0.5f, 0.5f));

        base.OnWalkEnter();
    }

    protected override void OnWalkStay()
    {
        base.OnWalkStay();
    }

    protected override void OnWalkExit()
    {
        //override beetle behaviour
    }

    protected override void OnAscendEnter()
    {
        transform.localEulerAngles = Vector3.zero;
        walkingRight = Player.main.X > transform.position.x;
        _targetVelocity.x = jumpHorizontalPower * (walkingRight ? 1 : -1);
        _targetVelocity.y = jumpVerticalPower;
    }

    protected override void OnAscendStay()
    {
        if (IsGrounded && _targetVelocity.y < 0)
        {
            ChangeStateTo(EnemyState.Idle);
        }
        MoveWithGravity(_targetVelocity, 1f);
    }

    protected override void OnStunExit()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    protected override bool CheckAttackCondition()
    {
        return playerInRange && playerInSight && playerInRoom;
    }


    protected void DetermineAction()
    {
        if (CheckAttackCondition())
        {
            ChangeStateTo(EnemyState.Ascend);
        }
        else
        {
            switch (CurrentState)
            {
                case EnemyState.Idle:
                    ChangeStateTo(EnemyState.Walk);
                    break;
                case EnemyState.Walk:
                    ChangeStateTo(EnemyState.Idle);
                    break;
                case EnemyState.Fly:
                case EnemyState.Ascend:
                case EnemyState.Descend:
                case EnemyState.Attack:
                case EnemyState.Die:
                case EnemyState.Stun:
                case EnemyState.Burn:
                default:
                    break;
            };
        }
    }

}
