using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using UnityEngine.UIElements;

public class Player : AgentMachine
{
    public enum Ability
    {
        Gun,
        Flight,
        Tractor,
        Boost
    }
    public enum PlayerState { Idle, Walk, Ascend, Hang, Descend, Fly, Boost, Stun, Burn }
    [SerializeField] private PlayerState _currentState;
    [SerializeField] private List<Ability> abilities = new();
    [SerializeField] private bool allAbilities = false;
    [SerializeField] private LayerMask groundedLayer;
    [SerializeField] private float _walkSpeed, _walkAccel, _walkFric, _jumpPower, _jumpDecayRate, _jumpInputAllowance, _coyoteSec, _airAccelPenalty, stunTimeMin = 1f;
    [SerializeField] private float _gravUp, _gravDown, _hangTime;
    [SerializeField] private float _airVertSpeed, _airHorSpeed, _airAccel, _airFric, _flightCost;
    [SerializeField] private float boostSpeed, boostDistance, boostCost, postBoostVelocityRate;
    [SerializeField] private Vector2 _targetVelocity;
    [SerializeField] private TractorBeam beam;
    [SerializeField] private float powerChargeTime;
    [SerializeField] private Meter powerMeter;
    private float _targetHorSpeed, _fric, _accel, _grav;
    private bool usingPower, outOfPower;
    private Vector3 boostDirection;
    private Vector3 stunDirection;
    private PlayerState returnFromBoost;
    private Alarm coyoteTime;
    private Alarm triedToJump;
    private Alarm stunMinAlarm;


    private float _currentGrav => _grav * Time.fixedDeltaTime;

    List<Rigidbody2D> externalVelocitySources = new();
    Vector2 oneFrameVelocity = Vector2.zero;

    private struct UserInput
    {
        public Vector2 direction;
        public bool jump;
        public bool shoot;
        public bool action;
    }
    private Vector2 _userInputDir;
    private bool[] _userInputAction = new bool[3];
    private UserInput lastInput;
    private void SetNewInputs()
    {
        UserInput inputs = new();
        inputs.direction = _userInputDir;
        inputs.jump = _userInputAction[0];
        inputs.shoot = _userInputAction[1];
        inputs.action = _userInputAction[2];

        lastInput = inputs;
        _userInputAction = new bool[3] { Input.GetButton("Jump"), Input.GetButton("Fire1"), Input.GetButton("Fire2") };
    }

    #region Events
    public Action<Ability> onAbilityUnlock;
    public Action onJumpPress;
    public Action onJumpHold;
    public Action onJumpRelease;
    public Action onShootPress;
    public Action onShootHold;
    public Action onShootRelease;
    public Action onActionPress;
    public Action onActionHold;
    public Action onActionRelease;
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
    public Action onStunEnter;
    public Action onStunStay;
    public Action onStunExit;
    public Action onBurnEnter;
    public Action onBurnStay;
    public Action onBurnExit;
    #endregion

    public bool IsGrounded => Physics2D.CapsuleCast(transform.position + (Vector3)_collider.offset, CapsuleCollider.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.02f, groundedLayer);
    public Rigidbody2D StandingOn => Physics2D.CapsuleCast(transform.position + (Vector3)_collider.offset, CapsuleCollider.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.02f, groundedLayer).rigidbody;
    public bool IsStunned => CurrentState == PlayerState.Stun;
    public Meter PowerMeter => powerMeter;
    public bool OutOfPower => outOfPower;
    public CapsuleCollider2D CapsuleCollider => _collider as CapsuleCollider2D;


    private WaitForFixedUpdate waitForFixedUpdate = new();
    public PlayerState CurrentState => _currentState;
    public Vector3 Position => transform.position;
    public float X => transform.position.x;
    public float Y => transform.position.y;
    public float Z => transform.position.z;
    public float Height => CapsuleCollider.size.y;
    public Vector2 UserInputDir => _userInputDir;
    public bool JumpPress => _userInputAction[0] && !lastInput.jump;
    public bool JumpHold => _userInputAction[0] && lastInput.jump;
    public bool JumpRelease => !_userInputAction[0] && lastInput.jump;
    public bool ShootPress => _userInputAction[1] && !lastInput.shoot;
    public bool ShootHold => _userInputAction[1] && lastInput.shoot;
    public bool ShootRelease => !_userInputAction[1] && lastInput.shoot;

    public bool ActionPress => _userInputAction[2] && !lastInput.action;
    public bool ActionHold => _userInputAction[2] && lastInput.action;
    public bool ActionRelease => !_userInputAction[2] && lastInput.action;

    public bool IsFlying => CurrentState == PlayerState.Fly;
    public bool BeamActive => beam.Active;
    public float PowerRemaining => powerMeter.Value;

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
        onActionPress += () => TryToBoost();
        coyoteTime = Alarm.Get(_coyoteSec, false, false);
        triedToJump = Alarm.Get(_jumpInputAllowance, false, false);
        stunMinAlarm = Alarm.Get(stunTimeMin, false, false);

        powerMeter.onMin += () => { outOfPower = true; };
        powerMeter.onMax += () => { outOfPower = false; };

        NextState();
    }
    private void Update()
    {
        _userInputAction[0] = Input.GetButton("Jump") || _userInputAction[0];
        _userInputAction[1] = Input.GetButton("Fire1") || _userInputAction[1];
        _userInputAction[2] = Input.GetButton("Fire2") || _userInputAction[2];
        _userInputDir = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _targetVelocity + oneFrameVelocity;
        oneFrameVelocity = Vector2.zero;
        externalVelocitySources.Clear();

        if (ShootPress)
            onShootPress?.Invoke();
        if (ShootHold)
            onShootHold?.Invoke();
        if (ShootRelease)
            onShootRelease?.Invoke();
        if (ActionPress)
            onActionPress?.Invoke();
        if (ActionHold)
            onActionHold?.Invoke();
        if (ActionRelease)
            onActionRelease?.Invoke();
        if (JumpPress)
            onJumpPress?.Invoke();
        if (JumpHold)
            onJumpHold?.Invoke();
        if (JumpRelease)
            onJumpRelease?.Invoke();

        if (!usingPower && CurrentState != PlayerState.Boost)
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

        if (JumpPress || triedToJump.IsPlaying)
            TryToJump(_jumpPower);
        //set our next state to...
        PlayerState nextState =
            UserInputDir.x != 0 ? PlayerState.Walk :
            !IsGrounded ? PlayerState.Descend :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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

        if (JumpPress || triedToJump.IsPlaying)
            TryToJump(_jumpPower);

        //set our next state to...
        PlayerState nextState =
            UserInputDir.x == 0 ? PlayerState.Idle :
            !IsGrounded ? PlayerState.Descend :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
        if (!JumpHold)
            _targetVelocity.y *= _jumpDecayRate;
        if (JumpPress)
            TryToFly();
        //set our next state to...
        PlayerState nextState =
            _targetVelocity.y <= 0 ? PlayerState.Hang :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
        Alarm hang = Alarm.GetAndPlay(_hangTime);
        hang.onComplete = () => ChangeStateTo(PlayerState.Descend);
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
        if (JumpPress)
            TryToFly();
        //set our next state to...
        PlayerState nextState =
            IsGrounded ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
        if (JumpPress)
            TryToFly();
        //set our next state to...
        PlayerState nextState =
            IsGrounded ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
            //PlayerState.Ascend
        }

        //set our next state to...
        PlayerState nextState =
            JumpPress ? PlayerState.Descend :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
        SetNewInputs();
    }

    /// <summary>
    /// Called once when exiting Boost state
    /// </summary>
    private void OnBoostExit()
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
        _targetVelocity = stunDirection;
        _targetVelocity.y = stunDirection.y;
    }

    /// <summary>
    /// Called every fixed update when in Stun state
    /// </summary>
    private void OnStunStay()
    {
        MoveVerticallyWithGravity();
        MoveHorizontallyWithInput();
        //set our next state to...
        PlayerState nextState =
            IsGrounded && stunMinAlarm.IsStopped ? PlayerState.Idle :
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
        SetNewInputs();
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
        SetNewInputs();
    }

    /// <summary>
    /// Called once when exiting Burn state
    /// </summary>
    private void OnBurnExit()
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

        _targetVelocity.x = Mathf.MoveTowards(_targetVelocity.x, _targetHorSpeed * UserInputDir.x, change * Time.fixedDeltaTime);

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

    public void AddVelocityForOneFrame(Vector2 velocity, Rigidbody2D rb)
    {
        if (externalVelocitySources.Contains(rb))
            return;
        oneFrameVelocity += velocity;
        externalVelocitySources.Add(rb);
    }

    private void AscendWithSpeed(float speed)
    {
        ChangeStateTo(PlayerState.Ascend);
        _targetVelocity.y = speed;
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

        if (CurrentState == PlayerState.Boost)
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
                powerMeter.SetNewBounds(0, powerMeter.Max + 5);
                powerMeter.Fill();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_targetVelocity.y > 0)
        {
            ContactPoint2D[] contactPoints = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contactPoints);
            foreach (var item in contactPoints)
            {
                if (item.point.y > transform.position.y && QMath.Difference(item.point.y, transform.position.y) > 0.45f)
                {
                    _targetVelocity.y = 0;
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent<Collectible>(out Collectible c))
        {
            if (c is AbilityUnlock)
            {
                var a = c as AbilityUnlock;
                UnlockAbility(a.Ability);
            }
            if (c is PowerUp)
            {
                var p = c as PowerUp;

            }
            c.Collect();
        }

        if (collision.TryGetComponent<EnemyMachine>(out EnemyMachine enemy))
        {
            ChangeStateTo(PlayerState.Stun);
            stunDirection.x = Mathf.Sign(transform.position.x - enemy.transform.position.x);
            stunDirection.y = IsGrounded ? 1 : 0;
            stunDirection *= enemy.ContactDamage;
        }
    }
}
