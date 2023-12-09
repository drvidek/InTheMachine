using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMachine : AgentMachine
{
    public enum EnemyState { Idle, Walk, Fly, Ascend, Descend, Attack, Die, Stun, Burn }
    [SerializeField] protected EnemyState _currentState;
    [SerializeField] protected float contactDamage;
    [SerializeField] protected float _fric;
    [SerializeField] protected float _grv;
    [SerializeField] protected float stunTimeMin;

    #region Events
    public Action onIdleEnter;
    public Action onIdleStay;
    public Action onIdleExit;
    public Action onWalkEnter;
    public Action onWalkStay;
    public Action onWalkExit;
    public Action onFlyEnter;
    public Action onFlyStay;
    public Action onFlyExit;
    public Action onAscendEnter;
    public Action onAscendStay;
    public Action onAscendExit;
    public Action onDescendEnter;
    public Action onDescendStay;
    public Action onDescendExit;
    public Action onPreAttack;
    public Action onAttackEnter;
    public Action onAttackStay;
    public Action onAttackExit;
    public Action onDieEnter;
    public Action onDieStay;
    public Action onDieExit;
    public Action onStunEnter;
    public Action onStunStay;
    public Action onStunExit;
    public Action onBurnEnter;
    public Action onBurnStay;
    public Action onBurnExit;
    #endregion

    public EnemyState CurrentState => _currentState;
    public float ContactDamage => contactDamage;
    public bool IsAttacking => CurrentState == EnemyState.Attack;

    protected override void Start()
    {
        base.Start();
        healthMeter.onMin += () => { ChangeStateTo(EnemyState.Die); };
        NextState();
    }

    #region State Machine
    /// <summary>
    /// Triggers the next state behaviour based on _currentState
    /// </summary>
    protected virtual void NextState()
    {
        //start the state coroutine based on the name of our _currentState enum
        StartCoroutine(_currentState.ToString());
    }
    /// <summary>
    /// Changes the state, triggering the current state's exit behaviour and the new state's entry behaviour
    /// </summary>
    protected virtual void ChangeStateTo(EnemyState state)
    {
        _currentState = state;
    }

    IEnumerator Idle()
    {
        //on entry
        OnIdleEnter();
        onIdleEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Idle)
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
    protected virtual void OnIdleEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Idle state
    /// </summary>
    protected virtual void OnIdleStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Idle state
    /// </summary>
    protected virtual void OnIdleExit()
    {

    }
    IEnumerator Walk()
    {
        //on entry
        OnWalkEnter();
        onWalkEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Walk)
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
    protected virtual void OnWalkEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Walk state
    /// </summary>
    protected virtual void OnWalkStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Walk state
    /// </summary>
    protected virtual void OnWalkExit()
    {

    }
    IEnumerator Fly()
    {
        //on entry
        OnFlyEnter();
        onFlyEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Fly)
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
    protected virtual void OnFlyEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Fly state
    /// </summary>
    protected virtual void OnFlyStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Fly state
    /// </summary>
    protected virtual void OnFlyExit()
    {

    }
    IEnumerator Ascend()
    {
        //on entry
        OnAscendEnter();
        onAscendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Ascend)
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
    protected virtual void OnAscendEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Ascend state
    /// </summary>
    protected virtual void OnAscendStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Ascend state
    /// </summary>
    protected virtual void OnAscendExit()
    {

    }
    IEnumerator Descend()
    {
        //on entry
        OnDescendEnter();
        onDescendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Descend)
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
    protected virtual void OnDescendEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Descend state
    /// </summary>
    protected virtual void OnDescendStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);
    }

    /// <summary>
    /// Called once when exiting Descend state
    /// </summary>
    protected virtual void OnDescendExit()
    {

    }
    IEnumerator Attack()
    {
        //on entry
        OnAttackEnter();
        onAttackEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Attack)
        {
            //state behaviour here

            OnAttackStay();
            onAttackStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnAttackExit();
        onAttackExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Attack state
    /// </summary>
    protected virtual void OnAttackEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Attack state
    /// </summary>
    protected virtual void OnAttackStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Attack state
    /// </summary>
    protected virtual void OnAttackExit()
    {

    }
    IEnumerator Die()
    {
        //on entry
        OnDieEnter();
        onDieEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Die)
        {
            //state behaviour here

            OnDieStay();
            onDieStay?.Invoke();
            //wait a frame
            yield return waitForFixedUpdate;
        }
        //on exit
        OnDieExit();
        onDieExit?.Invoke();
        //trigger the next state
        NextState();
    }

    /// <summary>
    /// Called once when entering Die state
    /// </summary>
    protected virtual void OnDieEnter()
    {
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        _targetVelocity = Vector2.zero;
    }

    /// <summary>
    /// Called every fixed update when in Die state
    /// </summary>
    private void OnDieStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Die state
    /// </summary>
    protected virtual void OnDieExit()
    {

    }
    IEnumerator Stun()
    {
        //on entry
        OnStunEnter();
        onStunEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Stun)
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
    protected virtual void OnStunEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Stun state
    /// </summary>
    protected virtual void OnStunStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Stun state
    /// </summary>
    protected virtual void OnStunExit()
    {

    }
    IEnumerator Burn()
    {
        //on entry
        OnBurnEnter();
        onBurnEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Burn)
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
    protected virtual void OnBurnEnter()
    {

    }

    /// <summary>
    /// Called every fixed update when in Burn state
    /// </summary>
    protected virtual void OnBurnStay()
    {
        //set our next state to...
        EnemyState nextState =
            //stay as we are
            _currentState;
        //trigger the next state
        ChangeStateTo(nextState);

    }

    /// <summary>
    /// Called once when exiting Burn state
    /// </summary>
    protected virtual void OnBurnExit()
    {

    }
    #endregion

    protected virtual void Move(Vector3 direction, float speed)
    {

    }

    protected virtual bool CheckAttackCondition()
    {
        return false;
    }

    protected virtual void TryToAttack()
    {
        if (CheckAttackCondition())
            ChangeStateTo(EnemyState.Attack);
    }

    /// <summary>
    /// Change state to Burning and instantiate a flame effect
    /// </summary>
    /// <param name="collider"></param>
    protected virtual void EnemyCatchFlame(Collider2D collider)
    {
        ChangeStateTo(EnemyState.Burn);
        burning = true;
        if (!burnEffect)
        {
            burnEffect = Instantiate(IFlammable.psysObjFire, transform);
            Instantiate(IFlammable.psysObjSmoke, burnEffect.transform);
        }
    }

    protected virtual void EnemyDouseFlame()
    {
        burning = false;
        if (burnEffect)
            IFlammable.ClearFireAndSmoke(burnEffect);
    }

}
