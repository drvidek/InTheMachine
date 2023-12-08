using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveAnimator : EnemyAnimator
{
    protected override void Start()
    {
        base.Start();

        myEnemy.onAttackEnter += () => animator.SetTrigger("Spawn");
    }
}
