using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Beetle : EnemyMachine, IProjectileTarget, IFlammable
{
    [SerializeField] protected Collider2D hardCollider;
    [SerializeField] protected float _walkSpeed;
    

    protected bool walkingRight = true;
    public bool WalkingRight => walkingRight;
    public Vector3 CurrentDirection => walkingRight ? transform.right : transform.right * -1;

    private BoxCollider2D boxCollider => _collider as BoxCollider2D;
    public Quaternion zRotation => Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward);

    public Vector3 NinetyDegrees => new Vector3(0f, 0f, 90f);

    public Vector2 groundBox => new Vector2(0.1f, boxCollider.size.y);
    public Vector2 wallBox => new Vector2(0.02f, 0.1f);

    public Collider2D StandingOn
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + (zRotation * (Vector3)boxCollider.offset), groundBox, transform.localEulerAngles.z, transform.up * -1, 0.02f, groundedMask);
            if (hit)
                return hit.collider;
            return null;
        }
    }

    public Collider2D ColliderAhead
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + (zRotation * (Vector3)boxCollider.offset), wallBox, 0, CurrentDirection, burning ? 0.2f : 0.02f, groundedMask);
            if (!hit)
                return null;
            return hit.collider;
        }
    }

    public bool IsGrounded => StandingOn != null;

    public int turnLock;

    protected override void Start()
    {
        base.Start();
        hardCollider.enabled = false;
    }

    protected override void OnWalkEnter()
    {
        hardCollider.enabled = false;
        walkingRight = QMath.Choose<bool>(true, false);
    }

    protected override void OnWalkStay()
    {
        turnLock = (int)Mathf.MoveTowards(turnLock, 0, 1);
        if (turnLock == 0)
        {
            //if we're not grounded
            if (!IsGrounded)
            {
                //rotate
                transform.localEulerAngles += walkingRight ? -NinetyDegrees : NinetyDegrees;
                transform.position = new Vector3(QMath.RoundToNearestFraction(transform.position.x, 1f / 4f), QMath.RoundToNearestFraction(transform.position.y, 1f / 4f), transform.position.z) + CurrentDirection * 0.02f;
                turnLock = 5;
            }
            else
            if (ColliderAhead)
            {
                transform.localEulerAngles += walkingRight ? NinetyDegrees : -NinetyDegrees;
                transform.position = new Vector3(QMath.RoundToNearestFraction(transform.position.x, 1f / 4f), QMath.RoundToNearestFraction(transform.position.y, 1f / 4f), transform.position.z) + CurrentDirection * 0.02f;
                turnLock = 5;
            }
        }

        Move(CurrentDirection, _walkSpeed);

        base.OnWalkStay();
    }

    protected override void OnWalkExit()
    {
        if (transform.localEulerAngles.z > 90 || transform.localEulerAngles.z < -90)
            transform.position += Vector3.down * 0.5f;
        transform.localEulerAngles = Vector3.zero;
    }

    protected override void OnStunEnter()
    {
        hardCollider.enabled = true;
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
        hardCollider.enabled = false;
    }

    protected override void OnBurnEnter()
    {
        transform.eulerAngles = Vector3.zero;
        hardCollider.enabled = true;
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

    public void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile)
        {
            DouseFlame();
            ChangeStateTo(EnemyState.Stun);
            _targetVelocity.x = Mathf.Sign(projectile.Direction.x) * projectile.Speed * 0.4f;
            _targetVelocity.y = projectile.Speed * 0.4f;
        }
        //if (projectile is FireProjectile)
        //{
        //    CatchFlame();
        //}
    }

    override protected void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    protected void MoveWithGravity(Vector3 direction, float speed)
    {
        _targetVelocity.x = direction.x * speed;
        if (!IsGrounded)
            _targetVelocity.y -= _grv * Time.fixedDeltaTime;
        else
            _targetVelocity.y = Mathf.Clamp(_targetVelocity.y, 0.01f, float.PositiveInfinity);
    }

    public  Rigidbody2D GetStandingOnRigidbody()
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
