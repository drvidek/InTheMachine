using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using System;
public class FireVent : MonoBehaviour, IActivate, IFlammable, IElectrocutable
{
    [SerializeField] private float length = 1;
    [SerializeField] private bool isLit;
    [SerializeField] private float timeToRelight;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private ParticleSystem psysFlame, psysGas, psysSpark;
    [SerializeField] private Sprite[] timerSprites;
    [SerializeField] private SpriteRenderer timer;
    private bool isLocked;
    private Alarm relightAlarm;

    public bool IsLit => isLit;

    private void OnValidate()
    {
        SetLength();
    }

    private void Start()
    {

        SetLength();
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

    private void SetLength()
    {
        if (!_collider)
            _collider = GetComponent<BoxCollider2D>();
        psysFlame.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _collider.size = new(_collider.size.x, length);
        _collider.offset = new Vector2(0, (length - 1) / 2);

        var main = psysFlame.main;
        main.startSpeed = length * 1.2f;
        main.duration = 0.25f / length;

        var gasMain = psysGas.main;
        gasMain.startSpeed = length;
        gasMain.duration = 0.25f / length;

        //make sure the system has stopped
    }

    private void FixedUpdate()
    {
        if (isLit)
            PropagateFlame(_collider);
        else
        {
            if (timeToRelight > 0)
            {
                int index = Mathf.RoundToInt(relightAlarm.PercentComplete * Mathf.Max(0, Mathf.FloorToInt(timerSprites.Length - 1)));
                timer.sprite = timerSprites[index];
            }
        }
    }

    public void CatchFlame(Collider2D collider)
    {
        if (isLocked)
            return;
        isLit = true;
        psysFlame.Play();
        psysGas.Stop();
        _collider.enabled = true;
        timer.sprite = timerSprites[^1];

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
        _collider.enabled = false;
        timer.sprite = timerSprites[0];

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

    public void RecieveElectricity(Collider2D collider)
    {
        CatchFlame(collider);
    }
}
