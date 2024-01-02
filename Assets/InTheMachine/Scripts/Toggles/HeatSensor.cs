using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
public class HeatSensor : LevelToggle, IFlammable
{
    [SerializeField] private float fillTime;
    private Meter activeMeter;

    private void Start()
    {
        activeMeter = new(0, 1, 0.1f);
        activeMeter.onMax += () => { if (!active) ToggleActive(true); };
        activeMeter.onMin += () => { if (active) ToggleActive(false); };
    }

    protected override void ToggleActive(bool active, bool force = false)
    {
        base.ToggleActive(active, force);
        animator.SetBool("Active", active);
    }

    private void FixedUpdate()
    {
        DouseFlame();
    }

    public void CatchFlame(Collider2D collider)
    {
        activeMeter.FillOver(fillTime / 2f);
    }

    public void DouseFlame()
    {
        activeMeter.EmptyOver(fillTime);
    }

    public void PropagateFlame(Collider2D collider)
    {

    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public bool IsFlaming()
    {
        return active;
    }
}
