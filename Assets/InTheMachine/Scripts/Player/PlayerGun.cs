using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class PlayerGun : Launcher
{
    [SerializeField] private GunProfile[] gunProfile;
    [SerializeField] private GunProfileType currentProfile;
    private Player myPlayer;
    private PlayerAnimate myAnimator;
    private bool canShoot = true;
    private GunProfileType lastProfile;
    private float cost;
    private bool costOnShot;

    public float Cost => cost;

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
        lastProfile = currentProfile;
        myPlayer.onShootPress += () => TryToShoot();
        SetProfile(currentProfile);

        myPlayer.onAbilityUnlock += UnlockPak;
    }

    private void Update()
    {
        currentProfile =
            Input.GetKeyDown(KeyCode.Alpha1) ? GunProfileType.Air :
            Input.GetKeyDown(KeyCode.Alpha2) ? GunProfileType.Fire :
            Input.GetKeyDown(KeyCode.Alpha3) ? GunProfileType.Air :
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

        if (costOnShot)
        {
            if (!Player.main.TryToUsePower(cost))
                return;
        }
        Projectile projectile = Instantiate(_projectilePrefab, SpawnPosition, Quaternion.identity, null);
        ApplyPropertiesToProjectile(projectile, direction);

    }

    protected override bool CanShoot()
    {
        return myPlayer.HasAbility(Player.Ability.Gun) && canShoot && myPlayer.PowerRemaining >= cost && !myPlayer.OutOfPower;
    }

    protected override Vector3 GetDirection()
    {
        if (Player.main.UserInputDir.y < 0.5f)
            return (Vector3)myAnimator.FacingDirection;
        return Vector3.up;
    }

    protected override void Reload()
    {

    }

    private void SetProfile(GunProfileType profile)
    {
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
    }

    private void UnlockPak(Player.Ability ability)
    {
        if (ability == Player.Ability.Gun)
        {
            myAnimator.EnablePak();
        }
    }
}
