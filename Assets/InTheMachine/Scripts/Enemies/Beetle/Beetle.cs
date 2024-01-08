using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Beetle : EnemyWalking, IFlammable
{
    [SerializeField] protected BoxCollider2D hardCollider;

    private BoxCollider2D boxCollider => _collider as BoxCollider2D;
    public Quaternion zRotation => Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward);

    public Vector3 NinetyDegrees => new Vector3(0f, 0f, 90f);

    public Vector2 groundBox => hardCollider.size;
    public Vector2 wallBox => new Vector2(hardCollider.size.x, hardCollider.size.y * 0.95f);

    public override bool IsGrounded => StandingOn != null;

    protected float cornerRounding = 4f;

    public override Collider2D StandingOn
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + (zRotation * (Vector3)hardCollider.offset), groundBox, transform.localEulerAngles.z, transform.up * -1, 0.02f, groundedMask);
            if (hit)
                return hit.collider;
            return null;
        }
    }

    public override Collider2D ColliderAhead
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + (zRotation * (Vector3)boxCollider.offset), wallBox, transform.localEulerAngles.z, CurrentDirection, burning ? 0.2f : 0.02f, groundedMask);
            if (!hit)
                return null;
            return hit.collider;
        }
    }

    public Collider2D EnemyAhead
    {
        get
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + (zRotation * (Vector3)boxCollider.offset), wallBox, transform.localEulerAngles.z, CurrentDirection, burning ? 0.2f : 0.02f, 1 << 7);
            foreach (var hit in hits)
            {
                if (hit.collider != hardCollider && hit.collider != _collider)
                    return hit.collider;
            }
            return null;

        }
    }


    public int turnLock;

    protected override void OnWalkEnter()
    {
        //hardCollider.enabled = false;
        transform.position = new Vector3(QMath.RoundToNearestFraction(transform.position.x, 1f / cornerRounding), QMath.RoundToNearestFraction(transform.position.y, 1f / cornerRounding), transform.position.z);
        walkingRight = QMath.Choose<bool>(true, false);
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
            RotateAroundCorner(walkingRight ? NinetyDegrees : -NinetyDegrees);
        }
        if (EnemyAhead)
            walkingRight = !walkingRight;

        Move(CurrentDirection, _walkSpeed);
    }

    protected virtual void RotateAroundCorner(Vector3 direction)
    {
        transform.localEulerAngles += direction; 
        transform.position = new Vector3(QMath.RoundToNearestFraction(transform.position.x, 1f / cornerRounding), QMath.RoundToNearestFraction(transform.position.y, 1f / cornerRounding), transform.position.z) + CurrentDirection * 0.2f;
        turnLock = 5;
    }

    protected override void OnWalkExit()
    {
        if (transform.localEulerAngles.z > 90 || transform.localEulerAngles.z < -90)
            transform.position += Vector3.down * 0.5f;

    }

    protected override void OnStunEnter()
    {
        transform.localEulerAngles = Vector3.zero;
        rb.velocity = _targetVelocity;
    }

    protected override void OnStunStay()
    {
        _targetVelocity = rb.velocity;
        _targetVelocity.x = Mathf.MoveTowards(_targetVelocity.x, 0, _fric * Time.fixedDeltaTime);
        _targetVelocity.y -= _grv * Time.fixedDeltaTime;
        if (IsGrounded && _targetVelocity.y < 0)
        {
            ChangeStateTo(EnemyState.Walk);
        }
        base.OnStunStay();
    }

    protected override void OnStunExit()
    {
        //hardCollider.enabled = false;
    }

    protected override void OnBurnEnter()
    {
        transform.eulerAngles = Vector3.zero;
        //hardCollider.enabled = true;
    }

    protected override void OnBurnStay()
    {
        PropagateFlame(_collider);

        if (ColliderAhead)
            walkingRight = !walkingRight;
        MoveWithGravity(CurrentDirection, _walkSpeed * 4);
        TakeDamage(1f * Time.fixedDeltaTime);
        base.OnBurnStay();
    }

    protected override void OnBurnExit()
    {
        DouseFlame();
        base.OnBurnExit();
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile)
        {
            DouseFlame();
            GetStunned(new Vector2(Mathf.Sign(projectile.Direction.x), 1), projectile.Speed * 0.4f);
            TakeDamage(projectile.Power);
            return;
        }
        if (projectile is ElecProjectile)
        {
            GetStunned(projectile.Direction, projectile.Power * 2f);
            TakeDamage(projectile.Power);
            return;
        }

        if (projectile is FireballProjectile)
        {
            CatchFlame(projectile.GetComponentInChildren<Collider2D>());
        }

        TakeDamage(projectile.Power);

    }

    override protected void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }



    override public Rigidbody2D GetStandingOnRigidbody()
    {
        if (!StandingOn)
            return null;
        return StandingOn.GetComponent<Rigidbody2D>();
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

    }

    public void CatchFlame(Collider2D flameSource)
    {
        EnemyCatchFlame(flameSource);
    }

    public void DouseFlame()
    {
        EnemyDouseFlame();
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

    public bool IsFlaming()
    {
        return CurrentState == EnemyState.Burn;
    }
}
