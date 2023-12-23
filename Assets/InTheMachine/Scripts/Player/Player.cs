using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Player : AgentMachine, IFlammable, IElectrocutable
{
    public enum Ability
    {
        Gun,
        Flight,
        Tractor,
        Boost,
        UltraBoost
    }
    public enum PlayerState { Idle, Walk, Ascend, Hang, Descend, Fly, Boost, UltraBoost, Stun, Burn, Heal }
    [SerializeField] private PlayerState _currentState;
    [SerializeField] private List<Ability> abilities = new();
    [SerializeField] private bool allAbilities = false;
    [SerializeField] private float _walkSpeed, _walkAccel, _walkFric, _jumpPower, _jumpDecayRate, _jumpInputAllowance, _coyoteSec, _airAccelPenalty, stunTimeMin = 1f;
    [SerializeField] private float _gravUp, _gravDown, _hangTime;
    [SerializeField] private float _airVertSpeed, _airHorSpeed, _airAccel, _airFric, _flightCost;
    [SerializeField] private float boostSpeed, boostDistance, boostCost, postBoostVelocityRate;
    [SerializeField] private TractorBeam beam;
    [SerializeField] private float powerChargeTime;
    [SerializeField] private Meter powerMeter;
    [SerializeField] private float iframeLength = 1f;
    [SerializeField] private float healDelay = 1f;
    [SerializeField] private Meter repairMeter;
    private float _targetHorSpeed, _fric, _accel, _grav;
    private bool usingPower, outOfPower;
    private Vector3 boostDirection;
    private PlayerState returnFromBoost;
    private Alarm coyoteTime;
    private Alarm triedToJump;
    private Alarm stunMinAlarm;
    private Alarm iframesAlarm;
    private Alarm healTimer;

    private Vector3 startPosition;

    private float _currentGrav => _grav * Time.fixedDeltaTime;

    private Vector2 _userInputDir;


    #region Events
    public Action<Ability> onAbilityUnlock;
    public Action onIdleEnter;
    public Action onIdleStay;
    public Action onIdleExit;
    public Action onWalkEnter;
    public Action onWalkStay;
    public Action onWalkExit;
    public Action onAscendEnter;
    public Action onAscendStay;
    public Action onAscendExit;
    public Action onHangEnter;
    public Action onHangStay;
    public Action onHangExit;
    public Action onDescendEnter;
    public Action onDescendStay;
    public Action onDescendExit;
    public Action onFlyEnter;
    public Action onFlyStay;
    public Action onFlyExit;
    public Action onBoostEnter;
    public Action onBoostStay;
    public Action onBoostExit;
    public Action onUltraBoostEnter;
    public Action onUltraBoostStay;
    public Action onUltraBoostExit;
    public Action onStunEnter;
    public Action onStunStay;
    public Action onStunExit;
    public Action onBurnEnter;
    public Action onBurnStay;
    public Action onBurnExit;
    public Action onHealEnter;
    public Action onHealStay;
    public Action onHealExit;
    #endregion


    public bool AllAbilities => allAbilities;

    public bool IsGrounded => Physics2D.CapsuleCast(transform.position + (Vector3)_collider.offset, CapsuleCollider.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.02f, groundedMask);
    public bool IsStunned => CurrentState == PlayerState.Stun;
    public bool IsHealing => CurrentState == PlayerState.Heal;
    public bool IsBoosting => CurrentState == PlayerState.Boost || CurrentState == PlayerState.UltraBoost;
    public bool IFramesActive => iframesAlarm.IsPlaying;
    public bool IsVulnerable => !IsBoosting && !IsStunned && !IFramesActive;
    public Meter PowerMeter => powerMeter;
    public bool OutOfPower => outOfPower;
    public CapsuleCollider2D CapsuleCollider => _collider as CapsuleCollider2D;

    public float CurrentHealth => healthMeter.Value;
    public float MaxHealth => healthMeter.Max;

    public PlayerState CurrentState => _currentState;
    public Vector3 Position => transform.position;
    public float X => transform.position.x;
    public float Y => transform.position.y;
    public float Z => transform.position.z;
    public float Height => CapsuleCollider.size.y;
    public Vector2 UserInputDir => _userInputDir;

    public FixedInput jump = new("Jump"), shoot = new("Shoot"), boost = new("Boost"), interact = new("Interact"), special = new("Special"), heal = new("Heal");

    public bool IsFlying => CurrentState == PlayerState.Fly;
    public bool BeamActive => beam.Active;
    public float PowerRemaining => powerMeter.Value;

    public float RepairMax => repairMeter.Max;
    public float RepairCurrent => repairMeter.Value;

    #region Singleton + Awake
    private static Player _singleton;
    public static Player main
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
                Debug.LogWarning("Player instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        main = this;
    }
    #endregion

    override protected void Start()
    {
        base.Start();
        _targetHorSpeed = _walkSpeed;
        boost.onPress += () => TryToBoost();
        coyoteTime = Alarm.Get(_coyoteSec, false, false);
        triedToJump = Alarm.Get(_jumpInputAllowance, false, false);
        stunMinAlarm = Alarm.Get(stunTimeMin, false, false);
        iframesAlarm = Alarm.Get(iframeLength, false, false);
        healTimer = Alarm.Get(healDelay, false, false);
        startPosition = transform.position;
        powerMeter.onMin += () => { outOfPower = true; };
        powerMeter.onMax += () => { outOfPower = false; };

        jump.onPress = JumpPressed;

        healthMeter.onMin += () =>
        {
            transform.position = Checkpoint.Current != null ? Checkpoint.Current.Position : startPosition;
            healthMeter.Fill();
        };

        heal.onPress += () =>
        {
            if (!IsStunned && !IsBoosting && !repairMeter.IsEmpty)
                ChangeStateTo(PlayerState.Heal);
        };

        healTimer.onComplete = () =>
        {
            repairMeter.Adjust(-1);
            healthMeter.Fill();
            ChangeStateTo(PlayerState.Idle);

        };


        NextState();
    }
    protected override void Update()
    {
        if (!GameManager.IsPlaying)
            return;
        base.Update();

        FixedInput.ReadAll();
        _userInputDir = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    override protected void FixedUpdate()
    {
        if (!GameManager.IsPlaying)
            return;
        base.FixedUpdate();

        if (!usingPower && !IsBoosting && !PlayerGun.main.DelayingShot)
            powerMeter.FillOver(powerChargeTime, false, true, true);
        usingPower = false;
    }

    #region State Machine
    /// <summary>
    /// Triggers the next state behaviour based on _currentState
    /// </summary>
    private void NextState()
    {
        //Debug.Log($"To State {CurrentState}");
        //start the state coroutine based on the name of our _currentState enum
        StartCoroutine(_currentState.ToString());
    }

    /// <summary>
    /// Changes the state, triggering the current state's exit behaviour and the new state's entry behaviour
    /// </summary>
    private void ChangeStateTo(PlayerState state)
    {
        _currentState = state;
    }

    IEnumerator Idle()
    {
        //on entry
        OnIdleEnter();
        onIdleEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Idle)
        {
            //state behaviour here

            OnIdleStay();
            onIdleStay?.Invoke();

            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnIdleExit();
        onIdleExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Idle state
    /// </summary>
    private void OnIdleEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Idle state
    /// </summary>
    private void OnIdleStay()
    {

        MoveVerticallyWithGravity();
        MoveHorizontallyWithInput();

        if (triedToJump.IsPlaying)
            TryToJump(_jumpPower);
        //set our next state to...
        PlayerState nextState =
            UserInputDir.x != 0 ? PlayerState.Walk :
            !IsGrounded ? PlayerState.Descend :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Idle state
    /// </summary>
    private void OnIdleExit()
    {
        if (!IsGrounded && CurrentState != PlayerState.Ascend)
            coyoteTime.ResetAndPlay();
    }
    IEnumerator Walk()
    {
        //on entry
        OnWalkEnter();
        onWalkEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Walk)
        {
            //state behaviour here

            OnWalkStay();
            onWalkStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnWalkExit();
        onWalkExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Walk state
    /// </summary>
    private void OnWalkEnter()
    {
        _targetHorSpeed = _walkSpeed;
        _fric = _walkFric;
        _accel = _walkAccel;
    }

    /// <summary>
    /// Called every fixed update when in Walk state
    /// </summary>
    private void OnWalkStay()
    {

        MoveHorizontallyWithInput();
        MoveVerticallyWithGravity();

        if (triedToJump.IsPlaying)
            TryToJump(_jumpPower);

        //set our next state to...
        PlayerState nextState =
            UserInputDir.x == 0 ? PlayerState.Idle :
            !IsGrounded ? PlayerState.Descend :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Walk state
    /// </summary>
    private void OnWalkExit()
    {
        if (!IsGrounded && CurrentState != PlayerState.Ascend)
            coyoteTime.ResetAndPlay();
    }
    IEnumerator Ascend()
    {
        //on entry
        OnAscendEnter();
        onAscendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Ascend)
        {
            //state behaviour here

            OnAscendStay();
            onAscendStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnAscendExit();
        onAscendExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Ascend state
    /// </summary>
    private void OnAscendEnter()
    {

        _grav = _gravUp;
    }

    /// <summary>
    /// Called every fixed update when in Ascend state
    /// </summary>
    private void OnAscendStay()
    {
        MoveHorizontallyWithInput();
        MoveVerticallyWithGravity();
        if (!jump.Hold)
            _targetVelocity.y *= _jumpDecayRate;
        //set our next state to...
        PlayerState nextState =
            _targetVelocity.y <= 0 ? PlayerState.Hang :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Ascend state
    /// </summary>
    private void OnAscendExit()
    {

    }

    IEnumerator Hang()
    {
        //on entry
        OnHangEnter();
        onHangEnter?.Invoke();
        Alarm hang = Alarm.Get(_hangTime); ;
        if (_hangTime > 0)
        {
            hang.Play();
            hang.onComplete = () =>
            {
                if (_currentState == PlayerState.Hang) ChangeStateTo(PlayerState.Descend);
                Debug.Log("HangAlarm complete");
            };
        }
        else
            ChangeStateTo(PlayerState.Descend);
        //every frame while we're in this state
        while (_currentState == PlayerState.Hang)
        {
            //state behaviour here

            OnHangStay();
            onHangStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnHangExit();
        onHangExit?.Invoke();
        hang.Release();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Hang state
    /// </summary>
    private void OnHangEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Hang state
    /// </summary>
    private void OnHangStay()
    {
        MoveHorizontallyWithInput();
        //set our next state to...
        PlayerState nextState =
            IsGrounded ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Hang state
    /// </summary>
    private void OnHangExit()
    {

    }
    IEnumerator Descend()
    {
        //on entry
        OnDescendEnter();
        onDescendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Descend)
        {
            //state behaviour here

            OnDescendStay();
            onDescendStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnDescendExit();
        onDescendExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Descend state
    /// </summary>
    private void OnDescendEnter()
    {
        _grav = _gravDown;
    }

    /// <summary>
    /// Called every fixed update when in Descend state
    /// </summary>
    private void OnDescendStay()
    {
        MoveHorizontallyWithInput();
        MoveVerticallyWithGravity();

        //set our next state to...
        PlayerState nextState =
            IsGrounded ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Descend state
    /// </summary>
    private void OnDescendExit()
    {

    }
    IEnumerator Fly()
    {
        //on entry
        OnFlyEnter();
        onFlyEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Fly)
        {
            //state behaviour here

            OnFlyStay();
            onFlyStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnFlyExit();
        onFlyExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Fly state
    /// </summary>
    private void OnFlyEnter()
    {
        _targetVelocity.y = 0;
        _targetHorSpeed = _airHorSpeed;
        _accel = _airAccel;
        _fric = _airFric;
    }

    /// <summary>
    /// Called every fixed update when in Fly state
    /// </summary>
    private void OnFlyStay()
    {
        MoveHorizontallyWithInput();
        MoveVerticallyWithInput();

        if (!TryToUsePower(_flightCost * Time.fixedDeltaTime))
            ChangeStateTo(PlayerState.Descend);

        if (IsGrounded && triedToJump.IsPlaying)
        {
            TryToJump(_jumpPower);
        }

        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Fly state
    /// </summary>
    private void OnFlyExit()
    {
        _targetHorSpeed = _walkSpeed;
        _accel = _walkAccel;
        _fric = _walkFric;
    }

    IEnumerator Boost()
    {
        //on entry
        OnBoostEnter();
        onBoostEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Boost)
        {
            //state behaviour here

            OnBoostStay();
            onBoostStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnBoostExit();
        onBoostExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Boost state
    /// </summary>
    private void OnBoostEnter()
    {
        boostDirection = PlayerAnimate.main.FacingDirection;
        Alarm boostEnd = Alarm.GetAndPlay(boostDistance / boostSpeed);
        boostEnd.onComplete = () => ChangeStateTo(returnFromBoost);
        _targetVelocity.y = 0f;
    }

    /// <summary>
    /// Called every fixed update when in Boost state
    /// </summary>
    private void OnBoostStay()
    {
        MoveInDirectionAtSpeed(boostDirection, boostSpeed);
        //set our next state to...
        PlayerState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Boost state
    /// </summary>
    private void OnBoostExit()
    {
        _targetVelocity *= postBoostVelocityRate;
        if (HasAbility(Ability.UltraBoost) && boost.Hold)
            ChangeStateTo(PlayerState.UltraBoost);
    }

    IEnumerator UltraBoost()
    {
        //on entry
        OnUltraBoostEnter();
        onUltraBoostEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.UltraBoost)
        {
            //state behaviour here

            OnUltraBoostStay();
            onUltraBoostStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnUltraBoostExit();
        onUltraBoostExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Boost state
    /// </summary>
    private void OnUltraBoostEnter()
    {
        //boostDirection = PlayerAnimate.main.FacingDirection;
        //Alarm boostEnd = Alarm.GetAndPlay(boostDistance / boostSpeed);
        //boostEnd.onComplete = () => ChangeStateTo(returnFromBoost);
        //_targetVelocity.y = 0f;
    }

    /// <summary>
    /// Called every fixed update when in Boost state
    /// </summary>
    private void OnUltraBoostStay()
    {
        MoveInDirectionAtSpeed(boostDirection, boostSpeed * 1.25f);
        if (!boost.Hold || !TryToUsePower(boostCost * 2f * Time.fixedDeltaTime))
            ChangeStateTo(PlayerState.Idle);
        //set our next state to...
        PlayerState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Boost state
    /// </summary>
    private void OnUltraBoostExit()
    {
        _targetVelocity *= postBoostVelocityRate;
    }
    IEnumerator Stun()
    {
        //on entry
        OnStunEnter();
        onStunEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Stun)
        {
            //state behaviour here

            OnStunStay();
            onStunStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnStunExit();
        onStunExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Stun state
    /// </summary>
    private void OnStunEnter()
    {
        stunMinAlarm.ResetAndPlay();
        rb.velocity = _targetVelocity;
    }

    /// <summary>
    /// Called every fixed update when in Stun state
    /// </summary>
    private void OnStunStay()
    {
        MoveVerticallyWithGravity();
        MoveHorizontallyWithInput();
        iframesAlarm.ResetAndPlay();
        //set our next state to...
        PlayerState nextState =
            IsGrounded && stunMinAlarm.IsStopped ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Stun state
    /// </summary>
    private void OnStunExit()
    {

    }
    IEnumerator Burn()
    {
        //on entry
        OnBurnEnter();
        onBurnEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Burn)
        {
            //state behaviour here

            OnBurnStay();
            onBurnStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnBurnExit();
        onBurnExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Burn state
    /// </summary>
    private void OnBurnEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Burn state
    /// </summary>
    private void OnBurnStay()
    {
        //set our next state to...
        PlayerState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Burn state
    /// </summary>
    private void OnBurnExit()
    {

    }

    IEnumerator Heal()
    {
        //on entry
        OnHealEnter();
        onHealEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == PlayerState.Heal)
        {
            //state behaviour here

            OnHealStay();
            onHealStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnHealExit();
        onHealExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Heal state
    /// </summary>
    private void OnHealEnter()
    {
        healTimer.ResetAndPlay();
    }

    /// <summary>
    /// Called every fixed update when in Heal state
    /// </summary>
    private void OnHealStay()
    {
        MoveVerticallyWithGravity();
        MoveHorizontallyWithInput();

        //set our next state to...
        PlayerState nextState =
            !heal.Hold ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        FixedInput.EatAll();
    }

    /// <summary>
    /// Called once when exiting Heal state
    /// </summary>
    private void OnHealExit()
    {

    }
    #endregion

    private void MoveHorizontallyWithInput()
    {
        float change = _fric;

        if (!IsStunned && UserInputDir.x != 0 && (IsGrounded || IsFlying))
            change = Mathf.Max(_accel, _fric);

        if (!IsGrounded && !IsFlying)
            change /= _airAccelPenalty;

        _targetVelocity.x = Mathf.MoveTowards(_targetVelocity.x, (IsStunned || IsHealing ? 0 : _targetHorSpeed) * UserInputDir.x, change * Time.fixedDeltaTime);

    }

    public void MoveInDirectionAtSpeed(Vector2 direction, float speed)
    {
        _targetVelocity = direction * speed;
    }

    private void MoveVerticallyWithInput()
    {
        float change = _fric;
        if (UserInputDir.y != 0)
            change = Mathf.Max(_accel, _fric);

        _targetVelocity.y = Mathf.MoveTowards(_targetVelocity.y, _airVertSpeed * UserInputDir.y, change * Time.fixedDeltaTime);
    }

    private void MoveVerticallyWithGravity()
    {
        _targetVelocity.y -= _currentGrav;
        if (IsGrounded)
            _targetVelocity.y = Mathf.Clamp(_targetVelocity.y, -0.05f, float.PositiveInfinity);

    }

    private void AscendWithSpeed(float speed)
    {
        ChangeStateTo(PlayerState.Ascend);
        _targetVelocity.y = speed;
    }

    private void JumpPressed()
    {
        switch (CurrentState)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
                TryToJump(_jumpPower);
                break;
            case PlayerState.Ascend:
            case PlayerState.Hang:
            case PlayerState.Descend:
                TryToFly();
                break;
            case PlayerState.Fly:

                ChangeStateTo(PlayerState.Descend);
                break;
            case PlayerState.Boost:
            case PlayerState.UltraBoost:
                break;
            case PlayerState.Stun:
            case PlayerState.Burn:
                break;
            default:
                break;
        }
    }

    private bool TryToJump(float speed)
    {
        if (!IsGrounded && coyoteTime.IsStopped && triedToJump.IsStopped)
        {
            triedToJump.ResetAndPlay();
            return false;
        }
        coyoteTime.Stop();
        triedToJump.Stop();
        AscendWithSpeed(speed);
        return true;
    }

    private bool TryToFly()
    {
        Debug.Log("Tried to fly");
        triedToJump.ResetAndPlay();

        if (coyoteTime.IsPlaying)
        {
            Debug.Log("Caught Coyote During Flight");
            TryToJump(_jumpPower);
            return false;
        }

        if (!HasAbility(Ability.Flight))
            return false;

        ChangeStateTo(PlayerState.Fly);
        return true;
    }

    public bool TryToUsePower(float power, bool checkMin = false)
    {
        if (outOfPower)
            return false;

        if (PowerRemaining < power && checkMin)
            return false;

        powerMeter.Adjust(-power);
        usingPower = true;
        return true;
    }

    private bool TryToBoost()
    {
        if (!HasAbility(Ability.Boost))
            return false;

        if (IsStunned || IsBoosting)
            return false;

        if (TryToUsePower(boostCost, false))
        {
            returnFromBoost = CurrentState;
            ChangeStateTo(PlayerState.Boost);
            return true;
        }
        return false;
    }

    public bool HasAbility(Ability ability)
    {
        return allAbilities || abilities.Contains(ability);
    }

    private void UnlockAbility(Ability ability)
    {
        abilities.Add(ability);
        onAbilityUnlock?.Invoke(ability);
    }

    private void AddPowerUp(PowerUp.Type type)
    {
        switch (type)
        {
            case PowerUp.Type.Power:
                powerMeter.SetNewBounds(0, powerMeter.Max + 10);
                powerMeter.Fill();
                break;
            default:
                break;
        }
    }

    public void RefillRepairCharges()
    {
        repairMeter.Fill();
    }

    protected override void CheckForExternalVelocity()
    {
        Rigidbody2D rb = GetStandingOnRigidbody();
        if (!rb)
            return;
        if (rb.velocity.magnitude == 0)
            return;
        if (externalVelocitySources.ContainsKey(rb))
            return;

        Debug.Log("Adding rigidbody");
        externalVelocitySources.Add(rb, rb.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_targetVelocity.y > 0)
        {
            ContactPoint2D[] contactPoints = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contactPoints);
            foreach (var item in contactPoints)
            {
                if (item.point.y > transform.position.y && QMath.Difference(item.point.y, transform.position.y) > 0.29f)
                {
                    _targetVelocity.y = 0;
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                    break;
                }
            }
        }

        if (CurrentState == PlayerState.UltraBoost && QMath.DoesLayerMaskContain(groundedMask, collision.gameObject.layer))
        {
            CheckBoostBreak(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CurrentState == PlayerState.UltraBoost && QMath.DoesLayerMaskContain(groundedMask, collision.gameObject.layer))
        {
            CheckBoostBreak(collision);
        }
    }

    private void CheckBoostBreak(Collision2D collision)
    {
        List<ContactPoint2D> list = new();
        collision.GetContacts(list);
        foreach (var contact in list)
        {
            if (Mathf.Sign(contact.point.x - _collider.bounds.center.x) == Mathf.Sign(rb.velocity.x) && contact.point.y > _collider.bounds.min.y)
            {
                ChangeStateTo(PlayerState.Idle);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.TryGetComponent<Collectible>(out Collectible c))
        {
            if (c is AbilityUnlock)
            {
                var a = c as AbilityUnlock;
                UnlockAbility(a.Ability);
                PopupText.main.DisplayAbilityText(a.Ability);
            }
            if (c is PowerUp)
            {
                var p = c as PowerUp;
                AddPowerUp(p.TypeHeld);
                PopupText.main.DisplayPowerupText(p.TypeHeld);
            }
            c.Collect();
        }

        if (IsVulnerable && collision.attachedRigidbody && collision.gameObject.layer == 7)
        {
            if (collision.attachedRigidbody.TryGetComponent<EnemyMachine>(out EnemyMachine enemy))
                TakeDamage(enemy.ContactDamage);
            if (collision.attachedRigidbody.TryGetComponent<Projectile>(out Projectile p))
                TakeDamage(p.Power);

            GetStunned(collision, 10f);
        }
    }

    public void GetStunned(Collider2D stunSource, float stunPower)
    {
        if (!IsVulnerable)
        {
            return;
        }
        ChangeStateTo(PlayerState.Stun);
        _targetVelocity.x = Mathf.Sign(transform.position.x - stunSource.transform.position.x);
        _targetVelocity.y = IsGrounded ? 1 : 0;
        _targetVelocity *= stunPower;
    }

    public Rigidbody2D GetStandingOnRigidbody()
    {
        return Physics2D.CapsuleCast(transform.position + (Vector3)_collider.offset, CapsuleCollider.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.02f, groundedMask).rigidbody;
    }

    public void PropagateFlame(Collider2D collider)
    {

    }

    public void PropagateFlame(Vector3 position, Vector2 size)
    {

    }

    public void CatchFlame(Collider2D collider)
    {
        if (IsVulnerable && collider.gameObject.layer != gameObject.layer)
        {
            TakeDamage(1f);
            GetStunned(collider, 10f);
        }
    }

    public void DouseFlame()
    {

    }

    public bool IsFlaming()
    {
        return false;
    }

    public void RecieveElectricity(Collider2D collider)
    {
        if (IsVulnerable && collider.gameObject.layer != gameObject.layer)
        {
            TakeDamage(1f);
            GetStunned(collider, 10f);
        }
    }
}
