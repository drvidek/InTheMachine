using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
public class FlyAnimator : EnemyAnimator
{

    protected override void Start()
    {
        base.Start();
        myEnemy.onFlyStay += () => { AgentAnimator.Vibrate(pixelAligner, QMath.Choose<int>(0, 0,1)); FaceMovingDirection(); };
        myEnemy.onIdleStay += () => AgentAnimator.Vibrate(pixelAligner, QMath.Choose<int>(0, 0, 1));
        myEnemy.onAttackStay += () => { AgentAnimator.Vibrate(pixelAligner, 1 * (myEnemy.rb.velocity == Vector2.zero ? 1 : 0)); FacePlayer(); };
        myEnemy.onStunEnter += () => { spriteRenderer.flipY = true; animator.SetFloat("Speed", 0); };
        myEnemy.onStunExit += () => { spriteRenderer.flipY = false; animator.SetFloat("Speed", 1); };
    }

    protected void FaceMovingDirection()
    {
        spriteRenderer.flipX = myAgent.rb.velocity.x < 0f;
    }

    protected void FacePlayer()
    {
        spriteRenderer.flipX = Player.main.Position.x < transform.position.x;
    }
}
