using System;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FungusCloud : Fungus
{
    [SerializeField] private float attackWindupTime, attackDuration;
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private ParticleSystem psysAttack;

    private Alarm attackWindupAlarm;
    private bool preAttackActive;


    protected override void Start()
    {
        attackWindupAlarm = alarmBook.AddAlarm("Attack",attackWindupTime, false);
        attackWindupAlarm.onComplete += () => { if (CurrentState == EnemyState.Idle) ChangeStateTo(EnemyState.Attack); };
        onPreAttack += OnPreAttack;
        base.Start();
    }
    protected override void OnIdleStay()
    {
        if (playerInRange && playerInSight && playerInRoom && !preAttackActive)
        {
            onPreAttack?.Invoke();
        }
        base.OnIdleStay();
    }

    protected override void OnIdleExit()
    {
        attackWindupAlarm.Stop();
        preAttackActive = false;
        base.OnIdleExit();
    }

    protected override void OnAttackEnter()
    {
        attackHitbox.enabled = true;
        psysAttack.Play();
        Alarm alarm = AlarmPool.GetAndPlay(attackDuration);
        alarm.onComplete = () => { if (IsAttacking) ChangeStateTo(EnemyState.Idle); };
        base.OnAttackEnter();
    }

    protected override void OnAttackExit()
    {
        //attackWindupAlarm.Stop();
        attackHitbox.enabled = false;
        base.OnAttackExit();
    }

    protected void OnPreAttack()
    {
        attackWindupAlarm.ResetAndPlay();
        preAttackActive = true;
    }
}
