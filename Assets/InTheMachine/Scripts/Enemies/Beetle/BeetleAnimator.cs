using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleAnimator : EnemyAnimator
{
    private Beetle myBeetle => myAgent as Beetle;

    protected override void Start()
    {
        base.Start();

        myEnemy.onWalkEnter += () => { animator.SetBool("Walking",true); };
        myEnemy.onWalkStay += () => { pixelAligner.SetOffset(transform.up * 0.25f); };
        myEnemy.onWalkExit += () => { animator.SetBool("Walking",false); };

        myEnemy.onStunEnter += () => { animator.SetBool("Stunned", true); pixelAligner.SetOffset(Vector2.zero); };
        myEnemy.onStunExit += () => { animator.SetBool("Stunned", false); };
        
        myEnemy.onBurnEnter += () => { pixelAligner.SetOffset(Vector2.up * 0.25f); };
    }

    private void Update()
    {
        spriteRenderer.flipX = !myBeetle.WalkingRight;
    }
}
