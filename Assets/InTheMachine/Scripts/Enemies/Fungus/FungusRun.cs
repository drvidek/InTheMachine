using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FungusRun : Fungus
{
    [SerializeField] private float walkSpeed, accel, jumpPower;

    private float currentSpeed, currentJumpPower;

    private Vector3 spawnPosition;

    private BoxCollider2D hardCollider;

    private Alarm returnAlarm;

    public float TargetXDirection => Mathf.Sign(QMath.Direction(transform.position, Player.main.Position).x);

    private float tempXDirection;

    public bool IsGrounded => StandingOn != null;

    public float CurrentSpeed => currentSpeed;
    public float MaxSpeed => walkSpeed;

    public virtual Collider2D StandingOn
    {
        get
        {
            RaycastHit2D[] hits = new RaycastHit2D[3];
            ContactFilter2D filter = new();
            filter.SetLayerMask(groundedMask);
            hardCollider.Cast(Vector2.down, filter, hits, 0.02f, true);
            Collider2D colliderFound = null;
            foreach (var hit in hits)
            {
                if (!hit)
                    continue;
                if (hit.collider != hardCollider)
                {
                    colliderFound = hit.collider;
                    break;
                }

            }
            return colliderFound;
        }
    }

    public virtual Collider2D ColliderAhead
    {
        get
        {
            //RaycastHit2D[] hits = new RaycastHit2D[3];
            //ContactFilter2D filter = new();
            //filter.SetLayerMask(groundedMask);

            //hardCollider.Cast(new(TargetXDirection, 0), filter, hits, 0.02f, true);
            Collider2D colliderFound = null;
            foreach (var hit in Physics2D.BoxCastAll(transform.position+(Vector3)hardCollider.offset,new(hardCollider.size.x, hardCollider.size.y * .8f),0,new(TargetXDirection,0),0.02f,groundedMask))
            {
                if (hit.collider != hardCollider)
                {
                    colliderFound = hit.collider;
                    break;
                }

            }
            return colliderFound;
        }

    }

    protected override void Start()
    {
        spawnPosition = transform.position;
        hardCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
        returnAlarm = alarmBook.AddAlarm("Return",5f, false);
        returnAlarm.onComplete = () =>
        {
            transform.position = spawnPosition;
            rb.velocity = Vector2.zero;
            _targetVelocity = Vector2.zero;
            ChangeStateTo(EnemyState.Idle);
        };
        base.Start();
    }

    protected override void OnIdleEnter()
    {
        hardCollider.enabled = false;
        returnAlarm.Stop();
        //rb.bodyType = RigidbodyType2D.Static;
        base.OnIdleEnter();
    }

    protected override void OnIdleStay()
    {
        if (CheckAttackCondition())
        {
            AscendAtSpeedInDirection(jumpPower, TargetXDirection);
        }
    }

    protected override void OnIdleExit()
    {
        hardCollider.enabled = true;
        //rb.bodyType = RigidbodyType2D.Dynamic;
        base.OnIdleExit();
    }

    protected override void OnAscendEnter()
    {
        currentSpeed = walkSpeed / 2f;
        _targetVelocity.y = currentJumpPower;
    }

    protected override void OnAscendStay()
    {
        MoveWithGravity(new(tempXDirection, 0), currentSpeed);
        if (_targetVelocity.y <= 0)
            ChangeStateTo(EnemyState.Descend);
    }

    protected override void OnDescendStay()
    {
        MoveWithGravity(new(tempXDirection, 0), currentSpeed);
        if (IsGrounded)
            ChangeStateTo(EnemyState.Walk);
    }

    protected override void OnWalkEnter()
    {
        _targetVelocity.y = 0;
        rb.velocity = _targetVelocity;
    }

    protected override void OnWalkStay()
    {
        float direction = Mathf.Sign(_targetVelocity.x);

        //turning around
        if (TargetXDirection != direction)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, _fric * Time.fixedDeltaTime);
            if (currentSpeed == 0)
            {
                direction = TargetXDirection;
                currentSpeed = 0.1f;
            }
        }
        else    //walking straight
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, accel * Time.fixedDeltaTime);
            if (ColliderAhead)
            {
                AscendAtSpeedInDirection(jumpPower * 1.5f, TargetXDirection * -1);
            }
        }

        if (!playerInRoom && !playerInSight)
        {
            if (returnAlarm.IsStopped)
                returnAlarm.ResetAndPlay();
        }
        else
        {
            returnAlarm.Stop();
        }

        MoveWithGravity(new(direction, 0), currentSpeed);
    }

    protected override void OnBurnStay()
    {
        float direction = Mathf.Sign(_targetVelocity.x);
        MoveWithGravity(new(direction, 0), currentSpeed);
        base.OnBurnStay();
    }

    private void AscendAtSpeedInDirection(float jumpSpeed, float xDirection)
    {
        currentJumpPower = jumpSpeed;
        tempXDirection = xDirection;
        ChangeStateTo(EnemyState.Ascend);
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
