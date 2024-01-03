using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    [SerializeField] Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();

        List<Collider2D> overlaps = new();
        ContactFilter2D contact = new();

        List<Tilemap> overlappedTilemaps = new();
        List<Vector3> pointsToDestroy = new();

        _collider.OverlapCollider(contact.NoFilter(), overlaps);

        foreach (var item in overlaps)
        {
            if (item.TryGetComponent<Tilemap>(out Tilemap t))
                overlappedTilemaps.Add(t);
            else
                if (item.gameObject.layer == 16)
            {
                pointsToDestroy.Add(item.transform.position);
                //Destroy(item.gameObject);
            }
        }

        foreach (var item in overlappedTilemaps)
        {
            if (item.gameObject.layer == 17)
                continue;
            foreach (var pos in pointsToDestroy)
            {
                item.SetTile(item.WorldToCell(pos), null);
            }
        }
    }
}
