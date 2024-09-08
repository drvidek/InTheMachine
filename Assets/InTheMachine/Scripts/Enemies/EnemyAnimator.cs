using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class EnemyAnimator : AgentAnimator
{
    protected EnemyMachine myEnemy => myAgent as EnemyMachine;


    protected override void Start()
    {
        base.Start();

        myEnemy.onDieEnter += () =>
        {
            spriteRenderer.enabled = false;
        };

        myEnemy.onDieExit += () =>
        {
            spriteRenderer.enabled = true;
        };
    }

}
