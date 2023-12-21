using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusSporeAnimator : EnemyAnimator
{
    protected override void Start()
    {
        base.Start();
        myEnemy.onIdleEnter += () =>
        {
            animator.ResetTrigger("PreAttack");
            animator.ResetTrigger("Attack");
        };
        myEnemy.onPreAttack += () => animator.SetTrigger("PreAttack");
        myEnemy.onAttackEnter += () => animator.SetTrigger("Attack");
    }
}
