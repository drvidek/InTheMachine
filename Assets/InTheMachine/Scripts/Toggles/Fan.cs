using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Fan : LevelToggle, IProjectileTarget
{
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    private Alarm activeAlarm;

    private void Start()
    {
        activeAlarm = Alarm.Get(duration, false, false);
        activeAlarm.Stop();
        activeAlarm.onComplete = () => ToggleActive(false);
    }

    private void Update()
    {
        animator.SetFloat("SpinSpeed", speed * activeAlarm.PercentRemaining);
    }

    public void OnProjectileHit(Projectile projectile)
    {
        ToggleActive(true);
    }

    protected override void ToggleActive(bool active, bool force = false)
    {
        if (active)
            activeAlarm.ResetAndPlay();
        base.ToggleActive(active);
    }

}
