using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Hive : MonoBehaviour
{
    [SerializeField] private Alarm spawnAlarm;
    [SerializeField] private EnemyMachine enemyToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool waitToSpawn;
    [SerializeField] private Animator animator;

    private EnemyMachine currentEnemy;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Alarm.Add(spawnAlarm);
        spawnAlarm.ResetAndPlay();
        spawnAlarm.onComplete += () =>
        {
            currentEnemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
            animator.SetTrigger("Spawn");
            if (waitToSpawn)
                currentEnemy.onDieEnter += spawnAlarm.ResetAndPlay;
            else
                spawnAlarm.ResetAndPlay();
        };
    }
}
