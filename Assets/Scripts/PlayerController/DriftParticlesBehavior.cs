using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PDriverController))]
public class DriftParticlesBehavior : MonoBehaviour
{
    public ParticleSystem[] driftParticles;
    public ParticleSystem.MinMaxGradient driftingColor;
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
        else if (p.drifting)
        {
            driftColor = driftingColor;
        }

        if (shouldPlay)
        {
            foreach (var q in driftParticles)
            {
                var colm = q.colorOverLifetime;
                colm.color = driftColor;
                q.Play();
            }
        }
        else
        {
            foreach (var q in driftParticles)
            {
                q.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
