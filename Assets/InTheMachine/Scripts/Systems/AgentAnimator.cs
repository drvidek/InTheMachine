using QKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAnimator : MonoBehaviour
{
    [SerializeField] protected AgentMachine myAgent;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected PixelAligner pixelAligner;

    protected Color myBaseColor;

    protected bool damageAnimationActive;

    protected float damageFlashSpeed = 0.1f;


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
        myBaseColor = spriteRenderer.color;

        myAgent.onTakeDamage += (float f) =>
        {
            if (damageAnimationActive || f == 0)
                return;

            damageAnimationActive = true;
            spriteRenderer.color = Color.red;
            Alarm alarmA = Alarm.GetAndPlay(damageFlashSpeed);
            alarmA.onComplete = () =>
            {
                spriteRenderer.color = myBaseColor;
                Alarm alarmB = Alarm.GetAndPlay(damageFlashSpeed);
                alarmB.onComplete = () => damageAnimationActive = false;
            };
        };
    }

    public static void Vibrate(PixelAligner pixelAligner, int amount)
    {
        float x = Random.Range(-amount, amount+1);
        float y = Random.Range(-amount, amount+1);
        pixelAligner.AddTempOffset(new(x, y));
    }
}
