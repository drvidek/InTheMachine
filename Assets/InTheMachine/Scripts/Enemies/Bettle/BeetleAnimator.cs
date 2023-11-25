using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleAnimator : AgentAnimator
{
    private EnemyMachine myEnemy => myAgent as EnemyMachine;
    protected override void Start()
    {
        base.Start();

        myEnemy.onWalkEnter += () => { animator.SetBool("Walking",true); };
        myEnemy.onWalkStay += () => { pixelAligner.SetOffset(transform.up * 0.5f); };
        myEnemy.onWalkExit += () => { animator.SetBool("Walking",false); };

        myEnemy.onStunEnter += () => { animator.SetBool("Stunned", true); };
        myEnemy.onStunExit += () => { animator.SetBool("Stunned", false); };
    }
}
