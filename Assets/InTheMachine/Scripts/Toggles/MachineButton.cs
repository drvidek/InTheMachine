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
#if UNITY_EDITOR
    public void SetStayPressed()
    {

        stayPressed = true;

    }
#endif
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
        Debug.Log($"Found {collision.gameObject.name}");

        if (!RoomManager.main.InSameRoom(transform, Player.main.transform))
            return;

        Debug.Log($"Same room as player");

        if (active && stayPressed)
            return;

        Debug.Log($"Not yet active");

        if (Physics2D.OverlapBox(transform.position, Vector2.one*.75f, 0, blockingLayer))
            return;

        Debug.Log($"No block");

        if (collision.isTrigger)
            return;

        Debug.Log($"Valid collision");

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
