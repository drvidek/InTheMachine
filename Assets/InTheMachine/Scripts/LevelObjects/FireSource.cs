using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
public class FireSource : MonoBehaviour, IActivate, IFlammable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ParticleSystem psysFlame, psysGas, psysSpark;
    [SerializeField] private bool isLit;
    [SerializeField] private float timeToRelight;
    private bool isLocked;
    private Alarm relightAlarm;

    public bool IsLit => isLit;

    private void Start()
    {
        if (timeToRelight > 0)
        {
            relightAlarm = Alarm.Get(timeToRelight, false, false);
            relightAlarm.onComplete += () =>
            {
                CatchFlame(_collider);
                psysSpark.Play();
            };
        }
        if (isLit)
            CatchFlame(_collider);
        else
            DouseFlame();
    }

    private void FixedUpdate()
    {
        if (isLit)
            PropagateFlame(_collider);
    }

    public void CatchFlame(Collider2D collider)
    {
        if (isLocked)
            return;
        isLit = true;
        psysFlame.Play();
        psysGas.Stop();
    }

    public void DouseFlame()
    {
        if (isLocked)
            return;
        isLit = false;
        psysGas.Play();
        psysFlame.Stop();
        psysFlame.Clear();
        relightAlarm?.ResetAndPlay();

    }

    public void PropagateFlame(Collider2D collider)
    {
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(collider))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame(collider);
        }
    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public void ToggleActive(bool active)
    {
        CatchFlame(_collider);
    }

    public void ToggleActiveAndLock(bool active)
    {
        CatchFlame(_collider);
        isLocked = true;
    }

    public bool IsFlaming()
    {
        return isLit;
    }
}
