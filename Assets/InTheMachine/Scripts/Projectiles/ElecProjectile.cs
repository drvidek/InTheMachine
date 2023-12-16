using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;
using UnityEngine.UIElements;

public class ElecProjectile : Projectile, IElectrocutable
{
    private Vector3 start, end;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public void RecieveElectricity(Collider2D collider)
    {

    }

    protected override void Start()
    {
        base.Start();
        start = transform.position;
        end = Hitscan();
        UpdateSpriteAndBox();
        Destroy(gameObject, _lifetime);
    }

    protected override void DoCollision(Collider2D collider)
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.zero;
        IElectrocutable.FindImmediateNeighbours(boxCollider, out List<IElectrocutable> list);
        foreach (var item in list)
        {
            item.RecieveElectricity(boxCollider);
        }
        spriteRenderer.flipY = QMath.Choose<bool>(true, false);
        spriteRenderer.flipX = QMath.Choose<bool>(true, false);
    }

    private void UpdateSpriteAndBox()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!boxCollider)
            boxCollider = GetComponentInChildren<BoxCollider2D>();

        float length = Vector3.Distance(start, end);

        spriteRenderer.size = new Vector2(length, 1);
        spriteRenderer.transform.localPosition = new Vector2((length - 1) / 2f + 0.5f, 0);
        boxCollider.size = new Vector2(length, .25f);
        boxCollider.offset = new Vector2((length - 1) / 2f + 0.5f, 0);

        transform.localEulerAngles = new(0, 0,
            Direction.y != 0 ? 90 :
            Direction.x < 0 ? 180 :
            0
            );
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
