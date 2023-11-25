using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Debris : MonoBehaviour, IProjectileTarget
{
    [SerializeField] private ParticleSystem psysDust;

    public void OnProjectileHit(float power)
    {
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        psysDust.Play();
        Destroy(gameObject, 1f);
    }
}
