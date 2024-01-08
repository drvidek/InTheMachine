using QKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMachine : AgentMachine, IProjectileTarget
{
    public enum EnemyState { Idle, Walk, Fly, Ascend, Descend, Attack, Die, Stun, Burn }
    [SerializeField] private bool showDebug;
    [SerializeField] protected EnemyState _currentState;
    [SerializeField] protected QuestID questID;
    [SerializeField] protected float contactDamage;
    [SerializeField] protected float _fric;
    [SerializeField] protected float _grv;
    [SerializeField] protected float stunTimeMin;
    [SerializeField] protected float attackRange;

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

    public Vector3 DirectionToPlayer => QMath.Direction(transform.position, Player.main.Position);
    protected float currentDistanceToPlayer => Vector3.Distance(transform.position, Player.main.Position);
    protected bool playerInRange => currentDistanceToPlayer < attackRange;
    virtual protected bool playerInSight => !Physics2D.Raycast(transform.position, DirectionToPlayer, currentDistanceToPlayer, groundedMask);
    protected bool playerInRoom => RoomManager.main.InSameRoom(transform, Player.main.transform);
    protected Vector3Int currentRoom => RoomManager.main.GetRoom(transform);

    public static GameObject psysSplat => Resources.Load("PSysSplat") as GameObject;

    protected bool everDoneLogic = false;

    protected Coroutine currentStateCoroutine = null;

    protected override void Start()
    {
        base.Start();
        RoomManager.main.onPlayerMovedRoom += CheckPlayerInRangeForLogic;
        healthMeter.onMin += () => { ChangeStateTo(EnemyState.Die); };
        NextState();
    }
    protected override void CheckPlayerInRangeForLogic(Vector3Int room)
    {
        if (everDoneLogic)
            doingLogic = RoomManager.main.PlayerWithinRoomDistance(transform);
        else
            doingLogic = RoomManager.main.PlayerWithinRoomDistance(transform, 0);

        everDoneLogic = doingLogic || everDoneLogic;

        if (CurrentState != EnemyState.Die)
        {
            rb.simulated = doingLogic;
            if (agentAnimator)
                agentAnimator.SetEnabled(doingLogic);
        }


    }
    #region State Machine
    /// <summary>
    /// Triggers the next state behaviour based on _currentState
    /// </summary>
    protected virtual void NextState()
    {
        //start the state coroutine based on the name of our _currentState enum
        currentStateCoroutine = StartCoroutine(_currentState.ToString());
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnIdleEnter();
        onIdleEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Idle)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;

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
        while (!DoingLogic)
            yield return null;
        OnWalkEnter();
        onWalkEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Walk)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        OnFlyEnter();
        onFlyEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Fly)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        OnAscendEnter();
        onAscendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Ascend)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnDescendEnter();
        onDescendEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Descend)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnAttackEnter();
        onAttackEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Attack)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnDieEnter();
        onDieEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Die)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        if (rb.bodyType != RigidbodyType2D.Static)
            Halt();
        if (questID != QuestID.Null)
            QuestManager.main.CompleteQuest(questID);

        if (burnEffect)
            IFlammable.ClearFireAndSmoke(burnEffect);

        Instantiate(psysSplat, transform.position, Quaternion.identity);
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnStunEnter();
        onStunEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Stun)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        while (!DoingLogic)
            yield return null;
        //on entry
        OnBurnEnter();
        onBurnEnter?.Invoke();
        //every frame while we're in this state
        while (_currentState == EnemyState.Burn)
        {
            //state behaviour here
            while (!DoingLogic)
                yield return null;
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
        if (healthMeter.IsEmpty)
        {

        }
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
        if (CurrentState == EnemyState.Die)
            return;
        ChangeStateTo(EnemyState.Burn);
        burning = true;
        if (!burnEffect)
        {
            burnEffect = IFlammable.InstantiateFireAndSmoke(transform);
        }
    }

    protected virtual void EnemyDouseFlame()
    {
        burning = false;
        if (burnEffect)
            IFlammable.ClearFireAndSmoke(burnEffect);
    }

    protected virtual void GetStunned(Vector2 direction, float power)
    {
        ChangeStateTo(EnemyState.Stun);
        _targetVelocity = direction * power;
        rb.velocity = _targetVelocity;
    }

    public abstract void OnProjectileHit(Projectile projectile);

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.isTrigger)
            return;
        if (!collision.rigidbody)
            return;

        if (collision.rigidbody.velocity == Vector2.zero)
            return;

        if (collision.rigidbody.GetComponent<EnemyMachine>())
            return;

        ContactFilter2D filter = new();

        filter.SetLayerMask(groundedMask);

        RaycastHit2D[] hits = new RaycastHit2D[3];

        if (_collider.Cast(collision.rigidbody.velocity.normalized, filter, hits, 0.05f) > 0)
        {
            foreach (var item in hits)
            {
                if (!item.rigidbody || (item.rigidbody != collision.rigidbody && !item.rigidbody.GetComponent<EnemyMachine>()))
                {
                    ChangeStateTo(EnemyState.Die);
                    break;
                }
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (showDebug && Player.main)
        {
            Gizmos.color = CheckAttackCondition() ? Color.green : !playerInRange ? Color.yellow : !playerInSight ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, Player.main.Position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, DirectionToPlayer, attackRange, groundedMask);
            if (hit)
            {
                Gizmos.DrawCube(hit.point, Vector3.one * 0.1f);
            }
        }
    }
}
