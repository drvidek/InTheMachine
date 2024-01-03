using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySpiderAnimator : EnemyAnimator
{
    protected BabySpider myBaby => myEnemy as BabySpider;

    protected override void Start()
    {
        base.Start();
        myEnemy.onWalkEnter += () => animator.SetBool("Walking", true);
        myEnemy.onWalkStay += () => spriteRenderer.transform.localEulerAngles = new(0, 0, Mathf.Round(myBaby.Direction.y) == 0 ? 90 : 0);
        myEnemy.onIdleEnter += () => animator.SetBool("Walking", false);
    }
}
