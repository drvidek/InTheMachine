using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class Debris : MonoBehaviour, IProjectileTarget
{
    [SerializeField] private ParticleSystem psysDust;

    public virtual void OnProjectileHit(Projectile projectile)
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
        Alarm alarm = AlarmPool.GetAndPlay(1f);
        alarm.onComplete = () => gameObject.SetActive(false);
        QuestManager.main.CompleteQuest(QuestID.Clean);
    }
}
