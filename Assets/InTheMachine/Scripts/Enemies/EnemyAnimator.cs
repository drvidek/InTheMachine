using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class EnemyAnimator : AgentAnimator
{
    protected EnemyMachine myEnemy => myAgent as EnemyMachine;


    protected override void Start()
    {
        base.Start();

        myEnemy.onDieEnter += () =>
        {
            spriteRenderer.enabled = false;
        };

        RoomManager.main.onPlayerMovedRoom += RoomCheck;
    }


    private void RoomCheck(Vector3Int room)
    {
        if (!animator)
            return;
        bool check = RoomManager.main.PlayerWithinRoomDistance(transform);
        animator.enabled = check;
    }
}
