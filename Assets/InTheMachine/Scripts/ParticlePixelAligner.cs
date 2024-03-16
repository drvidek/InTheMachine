using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePixelAligner : MonoBehaviour
{
    private ParticleSystem psys;
    private Vector3Int room;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        psys = GetComponent<ParticleSystem>();
        room = RoomManager.main.GetRoom(transform);
        RoomManager.main.onPlayerMovedRoom += SetActive;
    }

    private void SetActive(Vector3Int room)
    {
        active = (this.room == room);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (psys.isStopped || !active)
            return;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[psys.particleCount];
        psys.GetParticles(particles, psys.particleCount);
        for (int i = 0; i < psys.particleCount; i++)
        {
            particles[i].position = PixelAligner.RoundPositionToPixel(particles[i].position);
        }
    }
}
