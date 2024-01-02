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
    private bool playerStateAllowsShot = true;
    private bool delayingShot = false;
    private GunProfileType lastProfile;
    private float cost;
    private bool costOnShot;

    public Action<GunProfileType> onProfileChange;
    public Action<GunProfileType> onProfileUnlock;

    public bool PlayerStateAllowsShot => playerStateAllowsShot;
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
        myPlayer.onFlyEnter += () => playerStateAllowsShot = false;
        myPlayer.onFlyExit += () => playerStateAllowsShot = true;
        myPlayer.onBoostEnter += () => playerStateAllowsShot = false;
        myPlayer.onBoostExit += () => playerStateAllowsShot = true;
        myPlayer.onUltraBoostEnter += () => playerStateAllowsShot = false;
        myPlayer.onUltraBoostExit += () => playerStateAllowsShot = true;
        myPlayer.onStunEnter += () => playerStateAllowsShot = false;
        myPlayer.onStunExit += () => playerStateAllowsShot = true;
        lastProfile = currentProfile;

        SetCurrentProfile(currentProfile, myPlayer.AllAbilities);

        myPlayer.onAbilityUnlock += UnlockGun;
    }

    private void Update()
    {
        float swapH = Input.GetAxis("SwapH");
        float swapV = Input.GetAxis("SwapV");
        GunProfileType newProfile =
            swapV > 0 ? GunProfileType.Air :
             swapH < 0 ? GunProfileType.Fire :
            swapV < 0 ? GunProfileType.Elec :
            swapH > 0 ? GunProfileType.Goo :
            currentProfile;

        if (lastProfile != newProfile)
        {
            SetCurrentProfile(newProfile);
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

        if (!Player.main.TryToUsePower(cost * (costOnShot ? 1 : (1f / cost) * 2f)))//_lifetime / _speed / 2f)))
            return;

        Projectile projectile = Instantiate(_projectilePrefab, SpawnPosition, Quaternion.identity, null);
        ApplyPropertiesToProjectile(projectile, direction);

    }

    protected override bool CanShoot()
    {
        return myPlayer.HasAbility(Player.Ability.Gun) && playerStateAllowsShot && !delayingShot && !myPlayer.OutOfPower;
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
        Alarm delay = Alarm.GetAndPlay((1f/cost)*2f);
        delay.onComplete = () => delayingShot = false;
    }

    private void SetCurrentProfile(GunProfileType profile, bool force = false)
    {
        Debug.Log("Setting profile");

        if (!availableTypes.Contains(profile) || currentProfile == profile)
        {
            if (!force)
                return;
        }
        currentProfile = profile;
        GunProfile gun = gunProfile[(int)profile];
        _size = gun.size;
        _speed = gun.speed;
        _power = gun.power;
        _lifetime = gun.lifetime;
        _collidingLayer = gun.collidingLayer;
        _piercingLayer = gun.piercingLayer;
        _pinpointLayer = gun.pinpointLayer;
        _projectilePrefab = gun.projectilePrefab;
        cost = gun.cost;
        costOnShot = gun.costOnShot;

        myPlayer.shoot.onHold -= PlayerInputsShoot;
        myPlayer.shoot.onPress -= PlayerInputsShoot;

        if (costOnShot)
        {
            myPlayer.shoot.onPress += PlayerInputsShoot;
        }
        else
        {
            myPlayer.shoot.onHold += PlayerInputsShoot;
        }


        onProfileChange?.Invoke(profile);
    }

    private void PlayerInputsShoot()
    {
        TryToShoot();
    }

    private void UnlockGun(Player.Ability ability)
    {
        if (ability == Player.Ability.Gun)
        {
            myAnimator.EnablePak();
            UnlockGunType(GunProfileType.Air);
        }
    }

    public void UnlockGunType(GunProfileType type)
    {
        availableTypes.Add(type);
        onProfileUnlock?.Invoke(type);
        SetCurrentProfile(type, true);
    }
}
