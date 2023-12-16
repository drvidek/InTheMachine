using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLine : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LineRenderer line;

    private void OnValidate()
    {
        Initialise();
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (!line)
            line = GetComponent<LineRenderer>();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, float.PositiveInfinity, layerMask);
        if (hit)
        {
            line.positionCount = (int)hit.distance * GameManager.pixelsPerUnit;
            for (int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, new Vector3(i/ (float)GameManager.pixelsPerUnit, 0));
            }

        }
    }
}
