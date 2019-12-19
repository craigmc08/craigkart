using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PDriverController))]
public class DriftParticlesBehavior : MonoBehaviour
{
    public ParticleSystem[] turboChargedParticles;
    public ParticleSystem[] driftParticles;
    public ParticleSystem.MinMaxGradient miniturboChargeColor;
    public ParticleSystem.MinMaxGradient superturboChargeColor;
    public ParticleSystem.MinMaxGradient ultraturboChargeColor;

    PDriverController controller;
    private void Start()
    {
        controller = GetComponent<PDriverController>();
    }

    public void Drift(DriftParameters p)
    {
        bool shouldPlay = p.drifting && controller.Grounded;
        bool shouldPlayCharged = shouldPlay && (p.mtCharged || p.stCharged || p.utCharged);
        ParticleSystem.MinMaxGradient driftColor = default;
        if (p.utCharged)
        {
            driftColor = ultraturboChargeColor;
        }
        else if (p.stCharged)
        {
            driftColor = superturboChargeColor;
        }
        else if (p.mtCharged)
        {
            driftColor = miniturboChargeColor;
        }

        if (shouldPlayCharged)
        {
            StopParticles(driftParticles);
            foreach (var q in turboChargedParticles)
            {
                var colm = q.colorOverLifetime;
                colm.color = driftColor;
                q.Play();
            }
        } else if (shouldPlay)
        {
            StopParticles(turboChargedParticles);
            foreach (var q in driftParticles)
            {
                q.Play();
            }
        }
        else
        {
            StopParticles(driftParticles);
            StopParticles(turboChargedParticles);
        }
    }

    void StopParticles(ParticleSystem[] systems)
    {
        foreach (var q in systems)
        {
            q.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
