using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Fly : EnemyFlying, IFlammable, IElectrocutable
{
    [SerializeField] protected float idleTime;
    [SerializeField] protected float maxDistanceToNewDestination;
    [SerializeField] protected float maxDistanceFromHome = 10f;

    private Alarm stunAlarm;

    private Alarm respawnAlarm;

    protected Vector3 homePosition;
    protected CircleCollider2D circleCollider => _collider as CircleCollider2D;

    private Alarm idleAlarm;
    protected Vector3 targetDestination = Vector3.zero;

    protected override void Start()
    {
        homePosition = transform.position;
        idleAlarm = Alarm.Get(idleTime, false, false);
        idleAlarm.onComplete += () => { targetDestination = FindValidStraightLineTarget(); ChangeStateTo(EnemyState.Fly); };
        respawnAlarm = Alarm.Get(5f, false, false);
        respawnAlarm.onComplete += () =>
        {
            transform.position = homePosition;
            ChangeStateTo(EnemyState.Idle);
        };
        RoomManager.main.onPlayerMovedRoom += CheckRespawnTimerReset;
        base.Start();
    }

    private void CheckRespawnTimerReset(Vector3Int room)
    {
        if (IsFlaming())
            return;

        if (room == currentRoom)
        {
            respawnAlarm.Stop();
            return;
        }

        if (!RoomManager.main.InSameRoom(transform.position,homePosition) && !respawnAlarm.IsPlaying)
        {
            respawnAlarm.ResetAndPlay();
        }
    }


    protected override void OnIdleEnter()
    {
        base.OnIdleEnter();
        idleAlarm.ResetAndPlay();
        _targetVelocity = Vector2.zero;
    }

    protected override void OnIdleExit()
    {
        //if we exit idle for any reason make sure we're not on an alarm
        idleAlarm.Stop();
        base.OnIdleExit();
    }

    protected override void OnFlyStay()
    {
        if (Vector3.Distance(transform.position, targetDestination) > 0.05f)
        {
            Move(QMath.Direction(transform.position, targetDestination),
                moveSpeed * Mathf.Max(0.5f, Vector3.Distance(transform.position, targetDestination) / maxDistanceToNewDestination));

        }
        else
        {
            transform.position = targetDestination;
            ChangeStateTo(EnemyState.Idle);
        }
        base.OnFlyStay();
    }

    protected override void OnStunEnter()
    {
        stunAlarm = Alarm.GetAndPlay(stunTimeMin);
        stunAlarm.onComplete = () => { if (CurrentState == EnemyState.Stun) ChangeStateTo(EnemyState.Idle); };
    }

    protected override void OnStunStay()
    {
        _targetVelocity = rb.velocity;
        _targetVelocity.x = Mathf.MoveTowards(_targetVelocity.x, 0, _fric * Time.fixedDeltaTime);
        _targetVelocity.y -= _grv * Time.fixedDeltaTime;

        base.OnStunStay();
    }

    protected override void OnBurnStay()
    {
        _targetVelocity = rb.velocity;

        PropagateFlame(_collider);

        if (Random.Range(0f, 100f) > 80f)
        {
            targetDestination = RollNewDestination(transform.position, maxDistanceToNewDestination);
        }

        ApplyForce(QMath.Direction(transform.position, targetDestination), moveSpeed * 1.5f);
        TakeDamage(1f * Time.fixedDeltaTime);

        base.OnBurnStay();
    }

    protected override void OnBurnExit()
    {
        DouseFlame();
        base.OnBurnExit();
    }

    protected override void CheckForExternalVelocity()
    {

    }

    protected Vector3 FindValidStraightLineTarget()
    {
        //determine if we should start from our current position, or our home position
        Vector3 baseDestination = Vector3.Distance(transform.position, homePosition) > maxDistanceFromHome ? homePosition : transform.position;
        //Roll our new destination using our base position
        Vector3 newDestination = RollNewDestination(baseDestination, maxDistanceToNewDestination);
        int loops = 0;
        //while there is a block between our destinations, a block near our new destination, our destination is too far from home, or we're in our home room and our new destination is not
        while (Physics2D.Raycast(baseDestination, QMath.Direction(baseDestination, newDestination), Vector3.Distance(baseDestination, newDestination), groundedMask) ||
            Physics2D.OverlapCircle(newDestination, circleCollider.radius * 1.1f, groundedMask) ||
            Vector3.Distance(newDestination, homePosition) > maxDistanceFromHome ||
            RoomManager.main.InSameRoom(transform.position, homePosition) && !RoomManager.main.InSameRoom(homePosition, newDestination))
        {
            //roll a new destination
            newDestination = RollNewDestination(baseDestination, maxDistanceToNewDestination);
            loops++;
            if (loops > 100)
            {
                return transform.position;
            }
        }
        return newDestination;
    }

    protected Vector3 RollNewDestination(Vector3 baseDestination, float distance)
    {
        return baseDestination + (Vector3)Random.insideUnitCircle * Random.Range(1f, distance);
    }

    protected override void Move(Vector3 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    private void ApplyForce(Vector3 direction, float speed)
    {
        _targetVelocity += (Vector2)direction * speed * 0.25f;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (targetDestination != Vector3.zero)
            Gizmos.DrawLine(transform.position, targetDestination);
    }


    public override void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile)
        {
            DouseFlame();
            GetStunned(new Vector2(Mathf.Sign(projectile.Direction.x), 1), projectile.Speed * 0.4f);
            TakeDamage(projectile.Power);
            stunAlarm?.ResetAndPlay();
        }
        if (projectile is ElecProjectile)
        {
            GetStunned(Vector2.down, 2f);
            Instantiate(IFlammable.psysObjSmokePuff, transform.position, Quaternion.identity);
            TakeDamage(projectile.Power);
        }


        if (projectile is FireballProjectile)
        {
            CatchFlame(projectile.GetComponentInChildren<Collider2D>());
        }

        TakeDamage(projectile.Power);
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
        throw new System.NotImplementedException();
    }

    public void CatchFlame(Collider2D collider)
    {
        EnemyCatchFlame(collider);
    }

    public void DouseFlame()
    {
        EnemyDouseFlame();
    }
    public bool IsFlaming()
    {
        return CurrentState == EnemyState.Burn;
    }

    public void RecieveElectricity(Collider2D collider)
    {
        if (collider.gameObject.layer == 6)
            return;

        GetStunned(Vector2.down, 2f);
        Instantiate(IFlammable.psysObjSmokePuff, transform.position, Quaternion.identity);
        TakeDamage(5f);
    }
}
