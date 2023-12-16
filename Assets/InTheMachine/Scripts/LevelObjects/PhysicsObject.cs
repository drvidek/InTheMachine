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
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (sliding)
        {
            rb.velocity = slidingDirection * 20f;
        }

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
        if (sliding)
        {
            if (QKit.QMath.DoesLayerMaskContain(collidingMask, collision.gameObject.layer))
            {
                List<ContactPoint2D> list = new();
                collision.GetContacts(list);
                foreach (var contact in list)
                {
                    if (Mathf.Sign(contact.point.x - _collider.bounds.center.x) == slidingDirection.x && contact.point.y > _collider.bounds.min.y)
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
        rb.mass = 1f;
        sliding = true;
        slidingDirection.x = PlayerAnimate.main.FacingDirection.x;
    }

    private void EndSlide()
    {
        sliding = false;
        rb.velocity = Vector2.zero;
        slidingDirection = Vector2.zero;
        rb.gravityScale = 1;
        rb.mass = 1000f;
    }
}
