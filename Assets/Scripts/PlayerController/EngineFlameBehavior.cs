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
            if (p.turboing && !q.isPlaying) q.Play();
            else if (!p.turboing && q.isPlaying) q.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
