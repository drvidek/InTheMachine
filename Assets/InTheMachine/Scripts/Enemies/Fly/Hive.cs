using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Hive : EnemyStatic, IFlammable
{
    [SerializeField] private Alarm spawnAlarm;
    [SerializeField] private EnemyMachine enemyToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool waitToSpawn;
    [SerializeField] private Animator animator;

    private EnemyMachine currentEnemySpawned;


    protected override void Start()
    {
        Alarm.Add(spawnAlarm);
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
        spawnAlarm.SetTimeMaximum(0.5f);
        int max = QMath.Choose<int>(4, 5, 5, 6);
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
        TakeDamage(2f * Time.fixedDeltaTime);
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
        currentEnemySpawned = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);

    }

    protected void ManageSpawnTimerReset()
    {
        if (waitToSpawn)
            currentEnemySpawned.onDieEnter += spawnAlarm.ResetAndPlay;
        else
            spawnAlarm.ResetAndPlay();
    }
}