using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAnimator : MonoBehaviour
{
    [SerializeField] protected AgentMachine myAgent;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected PixelAligner pixelAligner;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!myAgent)
            myAgent = GetComponent<AgentMachine>();
        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!animator)
            animator = GetComponent<Animator>();
        if (!pixelAligner)
            pixelAligner = GetComponentInChildren<PixelAligner>();

    }

    public static void Vibrate(PixelAligner pixelAligner, int amount)
    {
        float x = Random.Range(-amount, amount+1);
        float y = Random.Range(-amount, amount+1);
        pixelAligner.AddTempOffset(new(x, y));
    }
}
