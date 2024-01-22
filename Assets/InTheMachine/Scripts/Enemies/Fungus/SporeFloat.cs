using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class SporeFloat : SporeProjectile
{
    public EnemyList.Type spawn;
    [SerializeField] private float descentSpeed = 1f;
    [SerializeField] private Collider2D hardCollider;
    [SerializeField] private AnimationCurve xSpeed;

    override protected void FixedUpdate()
    {
        _direction = rb.velocity;
        if (burning)
        {
            health -= Time.fixedDeltaTime;
            if (health <= 0)
            {
                IFlammable.InstantiateSmokePuff(transform);
                Destroy(gameObject);
            }
            PropagateFlame(_collider);
        }

        if (Direction.y <= 0)
        {
            _direction.x = xSpeed.Evaluate(Time.time) * 2f;// * Time.fixedDeltaTime;

            //hardCollider.enabled = false;
            spriteFlip += Time.fixedDeltaTime;
            if (spriteFlip > spriteFlipTime)
            {
                spriteFlip = 0;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            _direction.y = Mathf.MoveTowards(_direction.y, -descentSpeed, grav * Time.fixedDeltaTime);
        }
        else
        {
            _direction.x = Mathf.MoveTowards(_direction.x, 0, startHorSpeed * horStopRate * Time.fixedDeltaTime);
            _direction.y -= grav * Time.fixedDeltaTime;
        }
        MoveFixed();
    }

    protected override void MoveFixed()
    {
        rb.velocity = _direction;
    }

    protected override void DoCollision(Collider2D collider)
    {
        if (_direction.y > 0)
            return;

        if (!burning)
        {
            Vector3Int cell = RoomManager.main.environmentGrid.WorldToCell(transform.position);
            Vector3Int floor = cell;
            floor.y -= 1;
            Vector3 floorPos = RoomManager.main.environmentGrid.CellToWorld(floor) + RoomManager.main.environmentGrid.cellSize / 2f;
            Vector3 position = RoomManager.main.environmentGrid.CellToWorld(cell) + RoomManager.main.environmentGrid.cellSize / 2f;
            position.z = 0;
            EnemyList enemies = Resources.Load("EnemyList") as EnemyList;

            bool doNotSpawn = false;
            foreach (var item in Physics2D.OverlapPointAll(floorPos))
            {
                doNotSpawn = true;
                if (item.gameObject.layer == 10)
                {
                    doNotSpawn = false;
                    break;
                }
            }

            foreach (var item in Physics2D.OverlapPointAll(position, 1 << 7))
            {
                if (item != _collider && item != hardCollider)
                {
                    doNotSpawn = true;
                    break;
                }
            }
            if (!doNotSpawn)
            {
                Instantiate(enemies.enemyPrefabs[(int)spawn], position, Quaternion.identity);
                QuestManager.main.FailQuest(QuestID.Fungus);
            }
        }

        base.DoCollision(collider);
    }

}
