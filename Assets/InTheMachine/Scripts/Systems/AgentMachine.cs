using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public abstract class AgentMachine : MonoBehaviour
{
    
    [SerializeField] protected LayerMask groundedMask;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected AgentAnimator agentAnimator;

    [SerializeField] protected Meter healthMeter;

    protected WaitForFixedUpdate waitForFixedUpdate = new();

    protected Dictionary<Rigidbody2D,Vector2> externalVelocitySources = new();
    protected List<Rigidbody2D> lastExternalVelocitySources = new();
    protected Vector2 _targetVelocity = new();
    protected Vector2 oneFrameVelocity = new();

    public abstract Rigidbody2D GetStandingOnRigidbody();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!_rigidbody)
            _rigidbody = GetComponent<Rigidbody2D>();
        if (!_collider)
            _collider = GetComponent<Collider2D>();
        if (!agentAnimator)
            agentAnimator = GetComponent<AgentAnimator>();
       
    }

    protected virtual void Update()
    {
        CheckForExternalVelocity();
    }

    protected virtual void FixedUpdate()
    {
        _rigidbody.velocity = _targetVelocity + CalculateOneFrameVelocity();
        lastExternalVelocitySources.Clear();
        foreach (var item in externalVelocitySources)
        {
            lastExternalVelocitySources.Add(item.Key);
        }
        externalVelocitySources.Clear();
    }

    protected virtual Vector2 CalculateOneFrameVelocity()
    {
        Vector2 totalVelocity = new();
        foreach (var item in externalVelocitySources)
        {
            Vector2 newPosition = (Vector2)item.Key.transform.position;
            if (newPosition != item.Value)
            {
                totalVelocity += item.Key.velocity; 
                //double the value if first impact to catch up
                if (!lastExternalVelocitySources.Contains(item.Key))
                totalVelocity += item.Key.velocity; 
            }
        }
        return totalVelocity;
    }

    protected abstract void CheckForExternalVelocity();

    public void TakeDamage(float damage)
    {
        healthMeter.Adjust(-damage);
    }
}
