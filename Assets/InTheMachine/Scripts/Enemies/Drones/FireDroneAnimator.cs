using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class FireDroneAnimator : EnemyAnimator
{
    [SerializeField] private ParticleSystem psysFlamethrower, psysBurst;

    private FireDrone myDrone => myEnemy as FireDrone;

    protected override void Start()
    {

        base.Start();
        myEnemy.onPreAttack += () => animator.SetTrigger("PreAttack");

        myEnemy.onAttackExit += () =>
        {
            animator.SetBool("Attack", false);
            psysFlamethrower.Stop();
        };

        myDrone.onBurst += () =>
        {
            psysBurst.Play();
            animator.SetBool("Attack", true);
        };
        myDrone.onCharge += () => animator.SetBool("Attack", true);
        myDrone.onFlamethrower += () =>
        {
            animator.SetBool("Attack", true);
            psysFlamethrower.transform.localEulerAngles = new(0, 0, myDrone.CurrentDirection.x > 0 ? 0 : 180);
            psysFlamethrower.Play();
        };
        myDrone.onFireball += () => animator.SetBool("Attack", true);

    }

    private void Update()
    {
        spriteRenderer.flipX = !myDrone.WalkingRight;
    }
}
