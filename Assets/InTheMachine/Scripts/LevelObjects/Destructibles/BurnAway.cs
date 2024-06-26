using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using QKit;

public class BurnAway : MonoBehaviour, IFlammable, IProjectileTarget
{
    [SerializeField] protected QuestID quest;
    [SerializeField] protected Collider2D _collider;
    [SerializeField] private bool canExtinguish;
    [SerializeField] private float burnTime = 1f, catchTime = 0.5f;
    private bool isBurning;
    private GameObject burnEffect;
    private Alarm burnAlarm;
    private Tilemap tilemap;
    private Vector3Int cell;
    protected Meter catchFlame = new(0, 1, 0, 2, 1);
    
    private bool dying = false;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponentInChildren<Collider2D>();
        tilemap = GetComponentInParent<Tilemap>();
        cell = tilemap.WorldToCell(transform.position);
        transform.position = QMath.ReplaceVectorValue(transform.position, VectorValue.z, Player.main.Z);
        burnAlarm = new(burnTime, false);
        burnAlarm.Stop();
        InitialiseIFlammable();
    }

    protected void InitialiseIFlammable()
    {
        catchFlame.onMax = () =>
        {
            if (dying || isBurning)
                return;
            isBurning = true;
            burnEffect = Instantiate(IFlammable.psysObjFire, transform.position, Quaternion.identity);
            burnAlarm.ResetAndPlay();
        };
        burnAlarm.onComplete = EndOfLife;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isBurning)
        {
            catchFlame.EmptyOver(catchTime, true, true, true);
            return;
        }

        if (_collider)
        {
            PropagateFlame(_collider);
        }
        else
        {
            PropagateFlame(transform.position, Vector2.one);
        }
        burnAlarm.Tick(Time.fixedDeltaTime);
    }

    public void CatchFlame(Collider2D collider)
    {
        if (isBurning)
        {
            return;
        }

        if (collider.GetComponent<Fuse>())
        {
            catchFlame.Fill();
        }
        else
        {
            catchFlame.FillOver(catchTime, true, true, true);
        }
    }

    public void DouseFlame()
    {
        if (!canExtinguish)
            return;

        isBurning = false;
        if (burnEffect)
            IFlammable.ClearFireAndSmoke(burnEffect);
        burnAlarm.Stop();

    }

    public bool IsFlaming()
    {
        return isBurning;
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
        IFlammable thisFlam = GetComponentInChildren<IFlammable>();
        foreach (var flammable in IFlammable.FindFlammableNeighbours(position, size))
        {
            if (flammable != thisFlam)
                flammable.CatchFlame(null);
        }
    }


    public void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile || projectile is AirRocketProjectile)
            DouseFlame();
        if (projectile is FireProjectile || projectile is FireballProjectile)
            catchFlame.Fill();
    }

    private void EndOfLife()
    {
        IFlammable.ClearFireAndSmoke(burnEffect);
        if (quest != QuestID.Null)
            QuestManager.main.CompleteQuest(quest);
        if (tilemap)
            tilemap.SetTile(cell, null);
        else
            Destroy(gameObject);
    }
}
