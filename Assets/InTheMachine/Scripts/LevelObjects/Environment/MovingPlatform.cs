using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovingPlatform : EnvironmentBox, IActivate
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float ascendSpeed;
    [SerializeField] private float descendSpeed;
    private Rigidbody2D rb;
    private bool active;


    //public bool IsMoving => lastPosition != transform.position;

    protected override void SetCollider()
    {
        base.SetCollider();
        boxCollider.size = new Vector2(size.x - Mathf.Abs(0.1f * direction.y), size.y - Mathf.Abs(0.1f * direction.x));
    }

    override protected void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (direction.x == 0)
            rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionX;
        if (direction.y == 0)
            rb.constraints = rb.constraints | RigidbodyConstraints2D.FreezePositionY;
        // startPosition = transform.position;
    }

    private void Update()
    {
        //if (rb == Player.main.GetStandingOnRigidbody() && lastPosition != transform.position)
        //{
        //    Player.main.AddVelocityForOneFrame(rb.velocity, rb);
        //}
        //
        //lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
       
        rb.velocity = direction * (active ? ascendSpeed :
            //Vector3.Distance(transform.position, startPosition) <= 0.05f ? 0 :
            descendSpeed);

        //if (!active && Vector3.Distance(transform.position, startPosition) < 0.05f)
        //   transform.position = startPosition;
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
    }

    public void ToggleActiveAndLock(bool active)
    {
        this.active = active;
    }
}
