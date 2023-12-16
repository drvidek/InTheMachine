using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAnimator : EnemyAnimator
{
    protected override void Start()
    {
        base.Start();
        myEnemy.onAscendEnter += () =>
        {
            spriteRenderer.flipX = !(myEnemy as EnemyWalking).WalkingRight;
            animator.SetBool("Jumping", true);
        };
        myEnemy.onAscendExit += () => animator.SetBool("Jumping", false);
    }
}
