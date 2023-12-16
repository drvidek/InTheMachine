using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class ElectricitySource : MonoBehaviour, IActivate, IElectrocutable
{
    [SerializeField] private bool active;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask contactMask;

    private void OnValidate()
    {
        UpdateSpriteAndBox();
    }

    private void UpdateSpriteAndBox()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!boxCollider)
            boxCollider = GetComponentInChildren<BoxCollider2D>();

        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * -0.25f, transform.right, float.PositiveInfinity, contactMask);
        if (hit)
        {
            spriteRenderer.size = new Vector2(hit.distance, 1);
            spriteRenderer.transform.localPosition = new Vector2((hit.distance - 1) / 2f + 0.25f, 0);
            boxCollider.size = new Vector2(hit.distance, .25f);
            boxCollider.offset = new Vector2((hit.distance - 1) / 2f + 0.25f, 0);
        }
    }

    public void RecieveElectricity(Collider2D collider)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateSpriteAndBox();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            UpdateSpriteAndBox();
            IElectrocutable.FindImmediateNeighbours(boxCollider, out List<IElectrocutable> list);
            foreach (var item in list)
            {
                item.RecieveElectricity(boxCollider);
            }
            spriteRenderer.flipY = QMath.Choose<bool>(true, false);
            spriteRenderer.flipX = QMath.Choose<bool>(true, false);
        }
    }

    public void ToggleActive(bool active)
    {
        this.active = active;
        boxCollider.enabled = active;
        spriteRenderer.enabled = active;
    }

    public void ToggleActiveAndLock(bool active)
    {
        throw new System.NotImplementedException();
    }
}
