using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusJumpAnimator : EnemyAnimator
{
    protected override void Start()
    {
        base.Start();
        myEnemy.onAscendEnter += () => animator.SetTrigger("Ascend");
        myEnemy.onDescendEnter += () => animator.SetTrigger("Descend");
        myEnemy.onDescendExit += () =>
        {
            if (myEnemy.CurrentState == EnemyMachine.EnemyState.Idle)
                animator.SetTrigger("Idle");
        };
    }
}
