using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MachineButton : LevelToggle
{
    private List<Rigidbody2D> currentRigidbodies = new();
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool stayPressed;
    [SerializeField] private Sprite[] buttonSprites;
    [SerializeField] private LayerMask blockingLayer;

    public void SetStayPressed()
    {
#if UNITY_EDITOR
        stayPressed = true;
#endif
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //int col = stayPressed ? (int)ColorType.Button : (int)ColorType.ButtonLoose;
        //spriteRenderer.color = GameManager.currentColorPalette.color[col];
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

        if (Physics2D.OverlapBox(transform.position, Vector2.one*.75f, 0, blockingLayer))
            return;

        if (collision.isTrigger)
            return;


        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb)
        {
            if (!currentRigidbodies.Contains(rb))
                currentRigidbodies.Add(rb);
        }

        ToggleActive(currentRigidbodies.Count > 0);
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

        if (currentRigidbodies.Count == 0)

            ToggleActive(false);
    }

    override protected void ToggleActive(bool active, bool force = false)
    {
        if (this.active == active && !force)
            return;

        base.ToggleActive(active);
    }

    private void OnDrawGizmos()
    {
        int col = (int)ColorType.Button;
        Gizmos.color = GameManager.currentColorPalette.color[col];
        Gizmos.DrawCube(transform.position, Vector2.one * 0.5f);
    }
}
