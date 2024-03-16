using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FungusSpore : Fungus
{
    [SerializeField] private float attackDelay;
    [SerializeField] private Launcher launcher;
    private Alarm attackAlarm;

    private bool attackLock;

    protected override void Start()
    {
        attackAlarm = alarmBook.AddAlarm("Attack",attackDelay, false);
        attackAlarm.onComplete = () =>
        {
            if (CurrentState == EnemyState.Idle)
                ChangeStateTo(EnemyState.Attack);
        };
        base.Start();
    }


    protected override void OnIdleEnter()
    {

        base.OnIdleEnter();
    }

    protected override void OnIdleStay()
    {
        base.OnIdleStay();
        if (CheckAttackCondition())
        {
            attackLock = true;
            attackAlarm.ResetAndPlay();
            onPreAttack?.Invoke();
        }
    }

    protected override void OnAttackEnter()
    {
        launcher.TryToShoot();
        //ChangeStateTo(EnemyState.Idle);
    }


    protected override void OnAttackExit()
    {
        attackLock = false;
    }

    protected override void OnBurnEnter()
    {
        attackAlarm.Stop();
        base.OnBurnEnter();
    }

    protected override void OnBurnExit()
    {
        if (burning)
            catchFlame.Fill();
        base.OnBurnExit();
    }
    protected override bool CheckAttackCondition()
    {
        return playerInRange && playerInRoom && playerInSight && !attackLock;
    }
}
