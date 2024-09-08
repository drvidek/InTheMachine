using System.Drawing;
using QKit;
using UnityEngine;

public class Heart : EnemyStatic
{
    [SerializeField] private bool aggro;
    [SerializeField] GameObject[] attacks;
    [SerializeField] private RespawnObject[] enemySpawners;
    Material material;

    protected override void Start()
    {
        material = GetComponentInChildren<Renderer>().sharedMaterial;
        alarmBook.AddAlarm("Attack", 5f, false).Reset().onComplete = TryToAttack;
        alarmBook.AddAlarm("End", 5f, false).Reset().onComplete = EndAttack;
        alarmBook.AddAlarm("Hit", 0.1f, false).onComplete = () => material.SetFloat("_OnDamage", 0);

        onTakeDamage += (amount) =>
        {
            alarmBook.GetAlarm("Hit").ResetAndPlay();
        };


        base.Start();
    }

    protected override void TryToAttack()
    {
        Debug.Log($"{name} trying to attack");
        base.TryToAttack();
    }

    protected override bool CheckAttackCondition()
    {
        return true;
    }

    protected override void Update()
    {
        if (aggro)
        {
            foreach (var spawn in enemySpawners)
            {
                if (!spawn.ObjectCurrentlyInScene)
                {
                    spawn.ToggleActive(true);
                }
            }
        }
        base.Update();
        material.SetFloat("_DamageLerp", alarmBook.GetAlarm("Hit").PercentRemaining);
    }

    void EndAttack()
    {
        Debug.Log("EndAttacking");
        ChangeStateTo(EnemyState.Idle);
    }

    protected override void CheckPlayerInRangeForLogic(Vector3Int room)
    {
        doingLogic = RoomManager.main.PlayerWithinRoomDistance(transform, 1);
        //Debug.Log($"DoingLogic: {doingLogic}");
        everDoneLogic = doingLogic || everDoneLogic;

        if (CurrentState != EnemyState.Die)
        {
            rb.simulated = doingLogic;
            if (agentAnimator)
                agentAnimator.SetEnabled(doingLogic);
        }

        if (!doingLogic)
        {
            healthMeter.Fill();
            aggro = false;
            alarmBook.GetAlarm("Attack").Stop();
            EndAttack();
        }
    }


    protected override void OnAttackEnter()
    {
        attacks[Random.Range(0, attacks.Length)].SetActive(true);
        alarmBook.GetAlarm("End").ResetAndPlay();
    }

    protected override void OnAttackStay()
    {
        
    }

    protected override void OnAttackExit()
    {
        foreach (var attack in attacks)
        {
            attack.SetActive(false);
        }
        if (aggro)
        alarmBook.GetAlarm("Attack").ResetAndPlay();
    }

    public override void OnProjectileHit(Projectile projectile)
    {
        if (!aggro)
        {
            aggro = true;
            alarmBook.GetAlarm("Attack").ResetAndPlay();
        }
        TakeDamage(projectile.Power);
    }
}
