using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePixelAligner : MonoBehaviour
{
    private ParticleSystem psys;
    // Start is called before the first frame update
    void Start()
    {
        psys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[psys.particleCount];
        psys.GetParticles(particles, psys.particleCount);
        for (int i = 0; i < psys.particleCount; i++)
        {
            particles[i].position = PixelAligner.RoundPositionToPixel(particles[i].position);
        }
    }
}
