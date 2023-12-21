using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Debris : MonoBehaviour, IProjectileTarget
{
    [SerializeField] private ParticleSystem psysDust;

    public void OnProjectileHit(Projectile projectile)
    {
        if (projectile is AirProjectile)
        {
            EndOfLife();
        }
    }

    protected void EndOfLife()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        psysDust.Play();
        Destroy(gameObject, 1f);
        QuestManager.main.CompleteQuest(QuestID.Clean);
    }
}
