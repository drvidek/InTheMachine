using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class WallGoo : BurnAway, IElectrocutable
{
    public enum Connected
    {
        None,
        Left,
        Right,
        Both
    }

    [SerializeField] private Sprite[] gooSprites;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private Connected connected = Connected.None;

    public Quaternion zRotation => Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward);

    private void Start()
    {
        _collider = GetComponentInChildren<Collider2D>();
        base.InitialiseIFlammable();
    }

    public void Initialise()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position + zRotation * (Vector3)boxCollider.offset);
        foreach (var hit in colliders)
        {
            if (hit != boxCollider && (hit.gameObject.layer == 10 || hit.gameObject.layer == 18))
            {
                Destroy(gameObject);
                return;
            }
        }

        CheckConnections();
    }

    private void CheckConnections(bool checkNeighbours = true)
    {

        WallGoo leftGoo = null;
        WallGoo rightGoo = null;
        Vector3 centre = transform.position + zRotation * (Vector3)boxCollider.offset;

        RaycastHit2D[] leftHit = Physics2D.RaycastAll(centre, -transform.right, 1f, 1 << 18);
        foreach (var hit in leftHit)
        {
            if (hit.collider == boxCollider)
                continue;

            leftGoo = hit.rigidbody.GetComponent<WallGoo>();
        }

        RaycastHit2D[] rightHit = Physics2D.RaycastAll(centre, transform.right, 1f, 1 << 18);
        foreach (var hit in rightHit)
        {
            if (hit.collider == boxCollider)
                continue;

            rightGoo = hit.rigidbody.GetComponent<WallGoo>();
        }

        int connections = 0;

        if (leftGoo != null)
        {
            connected = Connected.Left;
            connections++;
            if (checkNeighbours)
                leftGoo.AddRight();
        }
        if (rightGoo != null)
        {
            connected = Connected.Right;
            connections++;
            if (checkNeighbours)
                rightGoo.AddLeft();
        }
        if (connections == 2)
        {
            connected = Connected.Both;
        }

        spriteRenderer.sprite = gooSprites[(int)connected];
    }

    private void AddLeft()
    {
        switch (connected)
        {
            case Connected.None:
                connected = Connected.Left;
                break;
            case Connected.Left:
                break;
            case Connected.Right:
                connected = Connected.Both;
                break;
            case Connected.Both:
                break;
            default:
                break;
        }
        spriteRenderer.sprite = gooSprites[(int)connected];
    }

    private void AddRight()
    {
        switch (connected)
        {
            case Connected.None:
                connected = Connected.Right;
                break;
            case Connected.Left:
                connected = Connected.Both;
                break;
            case Connected.Right:
                break;
            case Connected.Both:
                break;
            default:
                break;
        }
        spriteRenderer.sprite = gooSprites[(int)connected];
    }


    public void RecieveElectricity(Collider2D collider)
    {
        throw new System.NotImplementedException();
    }
}
