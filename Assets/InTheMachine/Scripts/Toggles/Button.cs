using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : LevelToggle
{
    private List<Rigidbody2D> currentRigidbodies = new();
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool stayPressed;
    
    [SerializeField] private Sprite[] buttonSprites;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        onActiveChanged.AddListener(AnimateButton);
    }

    private void AnimateButton(bool isActive)
    {
        int i = isActive ? 1 : 0;
        spriteRenderer.sprite = buttonSprites[i];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && stayPressed)
            return;

        if (collision.isTrigger)
            return;

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb)
        {
            if (!currentRigidbodies.Contains(rb))
                currentRigidbodies.Add(rb);
        }

        TriggerChange(currentRigidbodies.Count > 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (active && stayPressed)
            return;

        if (collision.isTrigger)
            return;

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb)
        {
            if (currentRigidbodies.Contains(rb))
                currentRigidbodies.Remove(rb);
        }

        TriggerChange(currentRigidbodies.Count == 0);
    }

    override protected void TriggerChange(bool active, bool force = false)
    {
        if (this.active == active && !force)
            return;

        base.TriggerChange(active);
    }
}
