using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class EnemyAnimator : AgentAnimator
{
    protected EnemyMachine myEnemy => myAgent as EnemyMachine;

    protected Color myBaseColor;

    protected bool damageAnimationActive;

    private float damageFlashSpeed = 0.1f;

    protected override void Start()
    {
        base.Start();
        myBaseColor = spriteRenderer.color;

        myEnemy.onDieEnter += () =>
        {
            spriteRenderer.enabled = false;
        };

        myAgent.onTakeDamage += (float f) =>
        {
            if (damageAnimationActive || f == 0)
                return;

            damageAnimationActive = true;
            spriteRenderer.color = Color.red;
            Alarm alarmA = Alarm.GetAndPlay(damageFlashSpeed);
            alarmA.onComplete = () =>
            {
                spriteRenderer.color = myBaseColor;
                Alarm alarmB = Alarm.GetAndPlay(damageFlashSpeed);
                alarmB.onComplete = () => damageAnimationActive = false;
            };
        };
    }
}
