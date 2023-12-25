using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusRunAnimator : EnemyAnimator
{
    [SerializeField] private float animRunSpeed = 1.5f;
    public FungusRun myFungus => myEnemy as FungusRun;

    protected override void Start()
    {
        base.Start();
        myEnemy.onAscendEnter += () => animator.SetBool("Active", true);

        myEnemy.onWalkEnter += () => animator.SetBool("Grounded", true);
        myEnemy.onIdleEnter += () =>
        {
            animator.SetBool("Active", false);
            animator.SetBool("Grounded", false);
        };
    }

    private void FixedUpdate()
    {
        float x = myEnemy.rb.velocity.x;
        if (x != 0)
            spriteRenderer.flipX = myFungus.TargetXDirection < 0;

        if (myEnemy.CurrentState == EnemyMachine.EnemyState.Walk)
            animator.SetFloat("Speed", (myFungus.CurrentSpeed / myFungus.MaxSpeed) * animRunSpeed);
    }
}
