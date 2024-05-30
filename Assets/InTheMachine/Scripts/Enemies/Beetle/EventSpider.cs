using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpider : Spider
{

    [SerializeField] private Transform spawnPoint;

    private bool forceLeft = true;

    public void ResetForEvent()
    {
        transform.position = spawnPoint.position;
        walkingRight = false;
        forceLeft = true;
        ChangeStateTo(EnemyState.Idle);
    }

    protected override void OnWalkEnter()
    {
        nextActionAlarm.ResetAndPlay(3f + Random.Range(-0.5f, 0.5f));
        if (!forceLeft)
            base.OnWalkEnter();
    }
}
