using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderNest : Hive
{

    protected override void OnAttackEnter()
    {
        if (CheckAttackCondition())
            base.OnAttackEnter();
        else
        {
            ManageSpawnTimerReset();
            ChangeStateTo(EnemyState.Idle);
        }
    }

    protected override bool CheckAttackCondition()
    {
        return playerInRoom;
     
    }
}
