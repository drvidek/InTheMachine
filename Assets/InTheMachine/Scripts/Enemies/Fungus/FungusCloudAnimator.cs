using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusCloudAnimator : EnemyAnimator
{
    protected override void Start()
    {
        base.Start();
        if (!animator)
            return;

        myEnemy.onIdleEnter += () =>
        {
            animator.ResetTrigger("PreAttack");
            animator.ResetTrigger("Attack");
        };
        myEnemy.onPreAttack += () => animator.SetTrigger("PreAttack");
        myEnemy.onAttackEnter += () => animator.SetTrigger("Attack");
    }
}
