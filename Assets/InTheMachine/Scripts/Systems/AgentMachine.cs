using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentMachine : MonoBehaviour
{
    [SerializeField] protected LayerMask groundedMask;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected AgentAnimator agentAnimator;

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

}
