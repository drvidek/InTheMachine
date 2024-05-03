using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask collidingMask;

    private Collider2D _collider;

    private Vector2 slidingDirection;
    private bool sliding;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        if (TryGetComponent<CompositeCollider2D>(out CompositeCollider2D col))
            _collider = col;
        else
            _collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (sliding && _collider is BoxCollider2D)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(_collider.bounds.center, _collider.bounds.size, 0, slidingDirection, Mathf.Abs(rb.velocity.x * Time.fixedDeltaTime), collidingMask);
            foreach (var hit in hits)
            {
                if (hit.collider != _collider)
                    EndSlide();
            }

        }

        if (sliding)
            rb.velocity = slidingDirection * 20f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody && collision.rigidbody.TryGetComponent<Player>(out Player p))
        {
            if (p.CurrentState == Player.PlayerState.UltraBoost)
            {
                ActivateSlide();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (sliding && _collider is CompositeCollider2D)
        {
            if (QKit.QMath.DoesLayerMaskContain(collidingMask, collision.gameObject.layer))
            {
                float forgiveness = _collider.bounds.size.y / 100f;
                List<ContactPoint2D> list = new();
                collision.GetContacts(list);
                foreach (var contact in list)
                {
                    Debug.Log(contact.point.y + $" {_collider.bounds.min.y + forgiveness}/{_collider.bounds.max.y - forgiveness}");
                    if (Mathf.Sign(contact.point.x - _collider.bounds.center.x) == slidingDirection.x &&
                        contact.point.y > _collider.bounds.min.y + forgiveness &&
                        contact.point.y < _collider.bounds.max.y - forgiveness)
                    {
                        EndSlide();
                        return;
                    }
                }
            }
        }

        if (collision.rigidbody && collision.rigidbody.TryGetComponent<Player>(out Player p))
        {
            if (p.CurrentState == Player.PlayerState.UltraBoost)
            {
                ActivateSlide();
            }
        }
    }

    private void ActivateSlide()
    {
        rb.gravityScale = 0;
        //rb.mass = 1f;
        sliding = true;
        slidingDirection.x = PlayerAnimate.main.FacingDirection.x;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void EndSlide()
    {
        sliding = false;
        rb.velocity = Vector2.zero;
        slidingDirection = Vector2.zero;
        rb.gravityScale = 1;
        //rb.mass = 1000f;
        QKit.Alarm alarm = QKit.AlarmPool.GetAndPlay(0.1f);
        alarm.onComplete = () => rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
}
