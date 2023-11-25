using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : EnvironmentBox
{
    private float yPos;

    private float playerHalfHeight;

    override protected void Start()
    {
        base.Start();
        SetYPos();
        playerHalfHeight = Player.main.Height / 2;
    }

    protected override void SetSprite()
    {
        base.SetSprite();
        sprite.size = new Vector2(size.x, size.y);
    }

    private void SetYPos()
    {
        yPos = transform.position.y + boxCollider.size.y / 2;
    }

    private void FixedUpdate()
    {
        boxCollider.enabled = Player.main.Y - playerHalfHeight > yPos;
    }
}
