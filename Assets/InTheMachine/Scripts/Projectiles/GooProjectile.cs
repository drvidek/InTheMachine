using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class GooProjectile : Projectile
{
    
    [SerializeField] float grav = 15f;
    [SerializeField] float gravDelay = 0.5f;
    [SerializeField] float fric = 15f;
    [SerializeField] private ParticleSystem psysTrail, psysSplash;
    [SerializeField] private WallGoo wallGoo;
    private bool dead;
    private float currentGrav;

    private Collider2D _collider;

    protected override void Start()
    {
        Alarm gravAlarm = Alarm.GetAndPlay(gravDelay);
        gravAlarm.onComplete = () => currentGrav = grav;
        if (_direction.y != 0)
            _direction.x = 0;
        _direction = Quaternion.AngleAxis(Random.Range(-4f, 4f), Vector3.forward) * _direction * _speed;
        base.Start();
    }


    private void FixedUpdate()
    {
        if (dead)
            return;
        _direction.y -= currentGrav * Time.fixedDeltaTime;
        _direction.x = Mathf.MoveTowards(_direction.x, 0, fric * Time.fixedDeltaTime);
        rb.velocity = Direction;
    }

    protected override void SetRigidbody()
    {
        _collider = GetComponentInChildren<Collider2D>();

        base.SetRigidbody();
        Alarm alarm = Alarm.GetAndPlay(gravDelay);
        alarm.onComplete = () => _collider.isTrigger = false;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
            return;

        if (!_collider.isTrigger)
            return;

        _collider.isTrigger = false;
        transform.position -= Direction*Time.fixedDeltaTime;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 10)
        {
            List<ContactPoint2D> contacts = new();
            collision.GetContacts(contacts);

            Vector3 furthest = Vector3.zero;
            float distance = 0f;


            foreach (var item in contacts)
            {
                Debug.Log($"Contact: {item.point}");
                float newDist = Vector3.Distance(rb.position, item.point);
                if (newDist > distance)
                {
                    furthest = item.point;
                    distance = newDist;
                }
            }

            Vector2 contactDirection = QMath.Direction(rb.position, furthest); //contacts[0].point;
            Debug.Log($"Contact {contactDirection}");
            contactDirection.Normalize();
            Vector2 hitDirection = new();
            hitDirection.x = Mathf.Abs(contactDirection.x) > Mathf.Abs(contactDirection.y) ? Mathf.Sign(contactDirection.x) : 0;
            hitDirection.y = Mathf.Abs(contactDirection.y) > Mathf.Abs(contactDirection.x) ? Mathf.Sign(contactDirection.y) : 0;

            RaycastHit2D hit = Physics2D.Raycast(rb.position, hitDirection, 0.5f, 1 << 10);
            if (hit)
            {
                Vector3Int cell = RoomManager.main.environmentGrid.WorldToCell(rb.position);
                Vector3 pos = RoomManager.main.environmentGrid.CellToWorld(cell) + (Vector3)(Vector2.one * 0.5f);
                WallGoo goo = Instantiate(wallGoo, RoomManager.main.environmentGrid.transform);
                goo.transform.position = pos;
                goo.transform.up = -hitDirection;
                psysSplash.transform.up = -hitDirection;
                goo.Initialise();
            }
            else
            {
                hitDirection.x = hitDirection.x == 0 ? Mathf.Sign(contactDirection.x) : 0;
                hitDirection.y = hitDirection.y == 0 ? Mathf.Sign(contactDirection.y) : 0;

                RaycastHit2D secondHit = Physics2D.Raycast(rb.position, hitDirection, 0.5f, 1 << 10);
                if (secondHit)
                {
                    Vector3Int cell = RoomManager.main.environmentGrid.WorldToCell(transform.position);
                    Vector3 pos = RoomManager.main.environmentGrid.CellToWorld(cell) + (Vector3)(Vector2.one * 0.5f);
                    WallGoo goo = Instantiate(wallGoo, RoomManager.main.environmentGrid.transform);
                    goo.transform.position = pos;
                    goo.transform.up = -hitDirection;
                    psysSplash.transform.up = -hitDirection;
                    goo.Initialise();
                }
            }

        }
        base.OnTriggerEnter2D(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    protected override void EndOfLife()
    {
        dead = true;
        psysSplash.Play();
        psysTrail.Stop();
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        rb.simulated = false;
        Destroy(gameObject, 1f);
    }
}
