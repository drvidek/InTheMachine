using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Spider : Beetle
{
    [SerializeField] private float actionDelay = 0.5f;
    [SerializeField] private float jumpVerticalPower, jumpHorizontalPower;

    protected Alarm nextActionAlarm;


    protected override void Start()
    {
        nextActionAlarm = alarmBook.AddAlarm("ActionDelay", actionDelay, false);
        nextActionAlarm.onComplete = DetermineAction;
        //cornerRounding /= 4f;
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
        turnLock = (int)Mathf.MoveTowards(turnLock, 0, 1);
        //if we're not grounded
        if (!IsGrounded)
        {
            if (turnLock > 0)
            {
                GetStunned(Vector3.down, 1f);
            }
            //rotate
            RotateAroundCorner(walkingRight ? -NinetyDegrees : NinetyDegrees);
        }
        else
        if (ColliderAhead)
        {
            bool door = (ColliderAhead.GetComponent<Door>());
            RotateAroundCorner(walkingRight ? NinetyDegrees : -NinetyDegrees);
            if (door)
                transform.position -= transform.up * (1f / cornerRounding);
        }
        if (EnemyAhead)
            walkingRight = !walkingRight;

        Move(CurrentDirection, _walkSpeed);
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
        switch (CurrentState)
        {
            case EnemyState.Idle:
                if (CheckAttackCondition())
                    ChangeStateTo(EnemyState.Ascend);
                else
                    ChangeStateTo(EnemyState.Walk);
                break;
            case EnemyState.Walk:
                if (CheckAttackCondition())
                    ChangeStateTo(EnemyState.Ascend);
                else
                    ChangeStateTo(EnemyState.Idle);
                break;
            case EnemyState.Descend:
            case EnemyState.Fly:
            case EnemyState.Ascend:
            case EnemyState.Attack:
            case EnemyState.Die:
            case EnemyState.Stun:
            case EnemyState.Burn:
            default:
                break;
        };
    }
}
