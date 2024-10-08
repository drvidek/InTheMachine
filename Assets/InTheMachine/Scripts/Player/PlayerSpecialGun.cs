using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using System;
using Machine;
public class PlayerSpecialGun : Launcher, IPersist
{

    [SerializeField] private SpecialGunProfile[] gunProfile;
    [SerializeField] private GunProfileType currentProfile;
    [SerializeField] private Meter charge;
    [SerializeField] private float rechargeTime;
    private Player myPlayer;

    public Alarm rechargeAlarm;
    public float CooldownPercent => rechargeAlarm.PercentComplete;
    public int ChargesAvailable => (int)charge.Value;
    public int ChargesMax => (int)charge.Max;

    protected bool canShoot => PlayerGun.main.PlayerStateAllowsShot;

    public Vector3 Direction => GetDirection();
    public GunProfileType CurrentProfile => currentProfile;

    public Vector3 SpawnPosition => PlayerGun.main.SpawnPosition;

    private void Awake()
    {
        myPlayer = GetComponent<Player>();
        myPlayer.special.onPress += () => TryToShoot(); //this is anon because TryToShoot returns a bool
        rechargeAlarm = new(rechargeTime, false);
        rechargeAlarm.Stop();
        rechargeAlarm.onComplete = IncreaseCharge;
    }

    private void Start()
    {
        PlayerGun.main.onProfileChange += SetCurrentProfile;
    }

    private void Update()
    {
        rechargeAlarm.Tick(Time.deltaTime);
    }

    protected override void Shoot()
    {

        Vector3 direction = GetDirection();
        if (!_projectilePrefab)
        {
            Debug.LogError($"No projectile prefab is installed in {gameObject.name}'s launcher.");
            return;
        }

        Projectile projectile = Instantiate(_projectilePrefab, SpawnPosition, Quaternion.identity, null);
        ApplyPropertiesToProjectile(projectile, direction);

    }

    private void SetCurrentProfile(GunProfileType profile)
    {

        currentProfile = profile;
        SpecialGunProfile gun = gunProfile[(int)profile];
        _size = gun.size;
        _speed = gun.speed;
        _power = gun.power;
        _lifetime = gun.lifetime;
        _collidingLayer = gun.collidingLayer;
        _piercingLayer = gun.piercingLayer;
        _pinpointLayer = gun.pinpointLayer;
        _projectilePrefab = gun.projectilePrefab;

    }

    protected override bool CanShoot()
    {
        return charge.Value > 0 && myPlayer.HasAbility(Player.Ability.Special);
    }

    protected override Vector3 GetDirection()
    {
        return PlayerGun.main.Direction;
    }

    protected override void Reload()
    {
        charge.Adjust(-1);
        if (!rechargeAlarm.IsPlaying)
            rechargeAlarm.ResetAndPlay();
    }

    private void IncreaseCharge()
    {
        charge.Adjust(1);
        if (!charge.IsFull)
            rechargeAlarm.ResetAndPlay();
    }

     public void Save()
    {
        DataPersistenceManager.Save<PlayerSpecialGun>(JsonUtility.ToJson(this, true));
    }

    public void Load()
    {
        if (DataPersistenceManager.TryToLoad<PlayerSpecialGun>(out string load))
        {
            JsonUtility.FromJsonOverwrite(load, this);
            SetCurrentProfile(CurrentProfile);
        }
    }
}