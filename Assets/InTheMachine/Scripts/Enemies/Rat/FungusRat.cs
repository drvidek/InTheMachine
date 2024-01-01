using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusRat : Rat, IFlammable
{
    protected bool active = false;

    protected override bool playerInSight
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position - (CurrentDirection * attackRange / 2f), CurrentDirection, attackRange, 1 << 6);
            return (hit.collider && hit.collider.GetComponent<Player>() && !Physics2D.Raycast(transform.position, QMath.Direction(transform.position, hit.point), currentDistanceToPlayer, groundedMask));
        }
    }

    protected override void OnIdleEnter()
    {
        if (CheckAttackCondition())
            ChangeStateTo(EnemyState.Attack);
    }

    protected override void OnIdleStay()
    {
        if (CheckAttackCondition())
            ChangeStateTo(EnemyState.Ascend);

        MoveWithGravity(_targetVelocity, 0);
    }

    protected override void OnAttackEnter()
    {
        base.OnIdleEnter();
    }

    protected override void OnAttackStay()
    {
        base.OnIdleStay();
    }

    protected override void ChooseDirection()
    {
        walkingRight = Player.main.X > transform.position.x;
    }

    protected override void OnBurnStay()
    {
        TakeDamage(2f * Time.fixedDeltaTime);
        PropagateFlame(_collider);
        MoveWithGravity(CurrentDirection, _walkSpeed);
        base.OnBurnStay();
    }

    protected override bool CheckAttackCondition()
    {
        return playerInSight;
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
        return burning;
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
}
