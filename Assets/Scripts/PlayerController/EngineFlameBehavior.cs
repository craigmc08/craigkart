using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFlameBehavior : MonoBehaviour
{
    public ParticleSystem[] nitroParticles;

    public void Turbo(TurboParameters p)
    {
        foreach (var q in nitroParticles)
        {
            if (p.turboing) q.Play();
            else q.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
