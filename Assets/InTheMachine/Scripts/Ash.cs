using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ash : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private GameObject debrisPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (QKit.QMath.DoesLayerMaskContain(mask,collision.gameObject.layer))
        {
            Vector3Int cell = RoomManager.main.environmentGrid.WorldToCell(transform.position);
            Vector3 pos = RoomManager.main.environmentGrid.CellToWorld(cell);
            //Instantiate(debrisPrefab,pos,)
        }
    }

}
