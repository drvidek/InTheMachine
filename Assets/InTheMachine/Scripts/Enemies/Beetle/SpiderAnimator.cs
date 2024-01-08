using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimator : EnemyAnimator
{
    private Spider mySpider => myAgent as Spider;

    protected override void Start()
    {
        base.Start();

        myEnemy.onWalkEnter += () => { animator.SetBool("Walking", true); };
        myEnemy.onWalkStay += () => { pixelAligner.SetOffset(transform.up * 0.25f); };
        myEnemy.onIdleEnter += () => { animator.SetBool("Walking", false); };
        myEnemy.onStunEnter += () => { animator.SetBool("Walking", false); };

        myEnemy.onAscendEnter += () => { animator.SetBool("Jumping", true); };
        myEnemy.onAscendStay += () => { pixelAligner.SetOffset(transform.up * 0.25f); };

        myEnemy.onAscendExit += () => { animator.SetBool("Jumping", false); };

        myEnemy.onBurnEnter += () => { pixelAligner.SetOffset(Vector2.up * 0.25f); };

    }

    private void Update()
    {
        spriteRenderer.flipX = !mySpider.WalkingRight;
    }
}
