using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Water : MonoBehaviour
{
    [SerializeField] private ParticleSystem psysSplash;

    private void Start()
    {
        Transform child = transform.GetChild(0);
        psysSplash.transform.localPosition = new(0, child.localScale.y / 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        psysSplash.transform.position = QMath.ReplaceVectorValue(psysSplash.transform.position, VectorValue.x, collision.attachedRigidbody.position.x);
        psysSplash.Play();
    }
}
