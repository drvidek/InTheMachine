using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyWalking : EnemyMachine
{
    [SerializeField] protected float _walkSpeed;

    Vector2 gravityDirection = Vector2.down;

    [SerializeField] protected bool walkingRight = true;
    public bool WalkingRight => walkingRight;
    public Vector3 CurrentDirection => walkingRight ? transform.right : transform.right * -1;
    public virtual bool IsGrounded => StandingOn != null;
    
    public virtual Collider2D StandingOn
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)_collider.offset, _collider.bounds.size, transform.localEulerAngles.z, gravityDirection, 0.02f, groundedMask);
            if (hit)
                return hit.collider;
            return null;
        }
    }

    public virtual Collider2D ColliderAhead
    {
        get
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _collider.bounds.size, transform.localEulerAngles.z, CurrentDirection, burning ? 0.2f : 0.02f, groundedMask);
            if (!hit)
                return null;
            return hit.collider;
        }
    }

    protected void MoveWithGravity(Vector3 direction, float speed)
    {
        _targetVelocity.x = direction.x * speed;
        if (!IsGrounded)
            _targetVelocity.y -= _grv * Time.fixedDeltaTime;
        else
            _targetVelocity.y = Mathf.Clamp(_targetVelocity.y, 0.01f, float.PositiveInfinity);
    }

    public abstract Rigidbody2D GetStandingOnRigidbody();

}
