using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Hive : EnemyStatic, IFlammable
{
    [SerializeField] protected float spawnDelay = 4f;
    [SerializeField] protected EnemyMachine enemyToSpawn;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected bool waitToSpawn;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int[] possibleBurnSpawnCounts;
    [SerializeField] protected float burnSpawnDelay = 0.5f;
    private EnemyMachine currentEnemySpawned;

    [SerializeField] private int maxSpawn = 6;
    private int currentSpawn;
    protected Alarm spawnAlarm;

    protected override void Start()
    {
        spawnAlarm = alarmBook.AddAlarm("Spawn", spawnDelay, false);
        spawnAlarm.ResetAndPlay();
        spawnAlarm.onComplete = () => ChangeStateTo(EnemyState.Attack);
        base.Start();
    }
    
    protected override void OnAttackEnter()
    {
        base.OnAttackEnter();

        SpawnEnemy();
        ManageSpawnTimerReset();

        ChangeStateTo(EnemyState.Idle);
    }

    protected override void OnBurnEnter()
    {
        spawnAlarm.SetTimeMaximum(burnSpawnDelay);
        int max = QMath.Choose<int>(possibleBurnSpawnCounts);
        int i = 0;
        spawnAlarm.onComplete = () =>
        {
            SpawnEnemy();
            if (i < max)
                ManageSpawnTimerReset();
            i++;
        };
        waitToSpawn = false;
        spawnAlarm.ResetAndPlay();

        if (currentEnemySpawned)
        {
            currentEnemySpawned.onDieEnter -= spawnAlarm.ResetAndPlay;
        }

        base.OnBurnEnter();
    }

    protected override void OnBurnStay()
    {
        PropagateFlame(_collider);
        TakeDamage(1.5f * Time.fixedDeltaTime);
        base.OnBurnStay();
    }

    protected override void OnBurnExit()
    {
        DouseFlame();
        base.OnBurnExit();
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

    }

    protected override void OnDieEnter()
    {
        spawnAlarm.Stop();
        base.OnDieEnter();
    }

    protected void SpawnEnemy()
    {
        if (currentSpawn >= maxSpawn && !IsFlaming())
            return;

        currentEnemySpawned = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        currentSpawn++;
        currentEnemySpawned.onDieEnter += () => currentSpawn--;

        QuestManager.main.FailQuest(QuestID.Pest);
    }

    protected void ManageSpawnTimerReset()
    {
        if (!IsAlive)
            return;
        if (waitToSpawn)
            currentEnemySpawned.onDieEnter += spawnAlarm.ResetAndPlay;
        else
            spawnAlarm.ResetAndPlay();
    }

    public override void OnProjectileHit(Projectile projectile)
    {

        if (projectile is AirProjectile)
            return;

        if (projectile is FireballProjectile)
        {
            CatchFlame(projectile.GetComponentInChildren<Collider2D>());
        }

        TakeDamage(projectile.Power);
    }
}
