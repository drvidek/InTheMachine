using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FungusRun : Fungus
{
    [SerializeField] private float walkSpeed, accel, jumpPower;
    private float currentSpeed;

    private float TargetDirection => Mathf.Sign(QMath.Direction(transform.position, Player.main.Position).x);

    private float tempDirection;

    public bool IsGrounded => StandingOn != null;

    public float CurrentSpeed => currentSpeed;
    public float MaxSpeed => walkSpeed;

    public virtual Collider2D StandingOn
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _collider.bounds.size, transform.localEulerAngles.z, Vector2.down, 0.02f, groundedMask);
            if (hit)
                return hit.collider;
            return null;
        }
    }

    public virtual Collider2D ColliderAhead
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _collider.bounds.size, transform.localEulerAngles.z, new(TargetDirection, 0), burning ? 0.2f : 0.02f, groundedMask);
            if (!hit)
                return null;
            return hit.collider;
        }
    
    }

    protected override void OnIdleStay()
    {
        if (CheckAttackCondition())
        {
            ChangeStateTo(EnemyState.Ascend);
        }
    }

    protected override void OnAscendEnter()
    {
        tempDirection = TargetDirection;
        currentSpeed = walkSpeed / 2f;
        _targetVelocity.y = jumpPower;
    }

    protected override void OnAscendStay()
    {
        MoveWithGravity(new(tempDirection, 0), currentSpeed);
        if (_targetVelocity.y <= 0)
            ChangeStateTo(EnemyState.Descend);
    }

    protected override void OnDescendStay()
    {
        MoveWithGravity(new(tempDirection, 0), currentSpeed);
        if (IsGrounded)
            ChangeStateTo(EnemyState.Walk);
    }

    protected override void OnWalkEnter()
    {
        _targetVelocity.y = 0;
    }

    protected override void OnWalkStay()
    {
        float direction = Mathf.Sign(_targetVelocity.x);
        if (TargetDirection != direction)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, _fric * Time.fixedDeltaTime);
            if (currentSpeed == 0)
            {
                direction = TargetDirection;
                currentSpeed = 0.1f;
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, accel * Time.fixedDeltaTime);
        }

        MoveWithGravity(new(direction, 0), currentSpeed);
    }


    protected void MoveWithGravity(Vector3 direction, float speed)
    {
        _targetVelocity.x = direction.x * speed;
        if (!IsGrounded)
            _targetVelocity.y -= _grv * Time.fixedDeltaTime;
        else
            _targetVelocity.y = Mathf.Clamp(_targetVelocity.y, 0.01f, float.PositiveInfinity);
    }

    protected override bool CheckAttackCondition()
    {
        return playerInRange && playerInRoom && playerInSight;
    }

    
}
