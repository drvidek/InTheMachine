using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Fan : LevelToggle, IProjectileTarget
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    private Alarm activeAlarm;

    private void Start()
    {
        activeAlarm = Alarm.Get(duration, false, false);
        activeAlarm.Stop();
        activeAlarm.onComplete = () => TriggerChange(false);
    }

    private void Update()
    {
        animator.SetFloat("SpinSpeed", speed * activeAlarm.PercentRemaining);
    }

    public void OnProjectileHit(float power)
    {
        TriggerChange(true);
    }

    protected override void TriggerChange(bool active, bool force = false)
    {
        if (active)
            activeAlarm.ResetAndPlay();
        base.TriggerChange(active);
    }

}
