using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Beetle : EnemyMachine, IProjectileTarget
{
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _fric;
    [SerializeField] protected float _grv;

    private BoxCollider2D boxCollider => _collider as BoxCollider2D;
    public Vector3 NinetyDegrees => new Vector3(0f, 0f, 90f);

    public bool IsGrounded => Physics2D.OverlapPoint(transform.position + (transform.up * -0.05f), groundedMask);
    public Rigidbody2D StandingOn => Physics2D.OverlapPoint(transform.position + (transform.up * -0.05f), groundedMask).GetComponent<Rigidbody2D>();


    protected override void OnWalkEnter()
    {

    }

    protected override void OnWalkStay()
    {
        //if we're not grounded
        if (!IsGrounded)
        {
            //rotate
            transform.localEulerAngles -= NinetyDegrees;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z) + transform.right * 0.02f;
        }
        else
        if (Physics2D.OverlapPoint(transform.position + transform.up * 0.5f, groundedMask))
        {
            transform.localEulerAngles += NinetyDegrees;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z) + transform.right * 0.02f;
        }

        Move(transform.right, _walkSpeed);

        base.OnWalkStay();
    }

    protected override void OnWalkExit()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    protected override void OnStunEnter()
    {

    }

    protected override void OnStunStay()
    {
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

    }

    public void OnProjectileHit(Projectile projectile)
    {
        //if (projectile.Power == 0)
        //{
        //    ChangeStateTo(EnemyState.Stun);
        //    _targetVelocity.x = projectile.Direction.x * projectile.Speed/3f;
        //    _targetVelocity.y = projectile.Speed / 3f;
        //}
        //if (projectile.Power == 1)
        //{
        //
        //}
    }

    override protected void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + (transform.up * -0.05f), 0.03f);
    }
}
