using System;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class PlayerGun : Launcher
{
    [SerializeField] private GunProfile[] gunProfile;
    [SerializeField] private GunProfileType currentProfile;
    [SerializeField] private List<GunProfileType> availableTypes = new();
    private Player myPlayer;
    private PlayerAnimate myAnimator;
    private bool canShoot = true;
    private bool delayingShot = false;
    private GunProfileType lastProfile;
    private float cost;
    private bool costOnShot;

    public Action<GunProfileType> onProfileChange;
    public Action<GunProfileType> onProfileUnlock;

    public Vector3 Direction => GetDirection();
    public float Cost => cost;
    public bool DelayingShot => delayingShot;
    public GunProfileType CurrentProfile => currentProfile;

    public Vector3 SpawnPosition
    {
        get
        {
            Vector3 dir = GetDirection();
            float x = _spawn.localPosition.x * myAnimator.FacingDirection.x;
            float y = _spawn.localPosition.y;
            if (dir.y > 0)
            {
                x = 0;
                y = _spawn.localPosition.y + PixelAligner.PixelsToWidth(myAnimator.PakAimUpOffset);
            }
            return transform.TransformPoint(new(x, y));
        }
    }

    #region Singleton + Awake
    private static PlayerGun _singleton;
    public static PlayerGun main
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("PlayerGun instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion


    private void Start()
    {
        myPlayer = GetComponent<Player>();
        myAnimator = GetComponent<PlayerAnimate>();
        myPlayer.onFlyEnter += () => canShoot = false;
        myPlayer.onFlyExit += () => canShoot = true;
        myPlayer.onBoostEnter += () => canShoot = false;
        myPlayer.onBoostExit += () => canShoot = true;
        myPlayer.onUltraBoostEnter += () => canShoot = false;
        myPlayer.onUltraBoostExit += () => canShoot = true;
        myPlayer.onStunEnter += () => canShoot = false;
        myPlayer.onStunExit += () => canShoot = true;
        lastProfile = currentProfile;

        SetProfile(currentProfile);

        myPlayer.onAbilityUnlock += UnlockPak;
    }

    private void Update()
    {
        currentProfile =
            Input.GetKeyDown(KeyCode.Alpha1) ? GunProfileType.Air :
            Input.GetKeyDown(KeyCode.Alpha2) ? GunProfileType.Fire :
            Input.GetKeyDown(KeyCode.Alpha3) ? GunProfileType.Elec :
            Input.GetKeyDown(KeyCode.Alpha4) ? GunProfileType.Air :
            currentProfile;

        if (lastProfile != currentProfile)
        {
            SetProfile(currentProfile);
            lastProfile = currentProfile;
        }
    }
    protected override void Shoot()
    {

        Vector3 direction = GetDirection();
        if (!_projectilePrefab)
        {
            Debug.LogError($"No projectile prefab is installed in {gameObject.name}'s launcher.");
            return;
        }

        if (!Player.main.TryToUsePower(cost * (costOnShot ? 1 : _lifetime / _speed)))
            return;

        Projectile projectile = Instantiate(_projectilePrefab, SpawnPosition, Quaternion.identity, null);
        ApplyPropertiesToProjectile(projectile, direction);

    }

    protected override bool CanShoot()
    {
        return myPlayer.HasAbility(Player.Ability.Gun) && canShoot && !delayingShot && !myPlayer.OutOfPower;
    }

    protected override Vector3 GetDirection()
    {
        if (Player.main.UserInputDir.y < 0.5f)
            return (Vector3)myAnimator.FacingDirection;
        return Vector3.up;
    }

    protected override void Reload()
    {
        if (costOnShot)
            return;
        delayingShot = true;
        Alarm delay = Alarm.GetAndPlay(_lifetime/_speed);
        delay.onComplete = () => delayingShot = false;
    }

    private void SetProfile(GunProfileType profile)
    {
        if (!availableTypes.Contains(profile))
            return;

        GunProfile gun = gunProfile[(int)profile];
        _size = gun.size;
        _speed = gun.speed;
        _power = gun.power;
        _lifetime = gun.lifetime;
        _collidingLayer = gun.collidingLayer;
        _piercingLayer = gun.piercingLayer;
        _projectilePrefab = gun.projectilePrefab;
        cost = gun.cost;
        costOnShot = gun.costOnShot;

        if (costOnShot)
        {
            myPlayer.onShootPress += PlayerInputsShoot;
            myPlayer.onShootHold -= PlayerInputsShoot;
        }
        else
        {
            myPlayer.onShootPress -= PlayerInputsShoot;
            myPlayer.onShootHold += PlayerInputsShoot;
        }


        onProfileChange?.Invoke(profile);
    }

    private void PlayerInputsShoot()
    {
        TryToShoot();
    }

    private void UnlockPak(Player.Ability ability)
    {
        if (ability == Player.Ability.Gun)
        {
            myAnimator.EnablePak();
            UnlockGunType(GunProfileType.Air);
        }
    }

    private void UnlockGunType(GunProfileType type)
    {
        availableTypes.Add(type);
        onProfileUnlock?.Invoke(type);
        SetProfile(type);
    }
}
