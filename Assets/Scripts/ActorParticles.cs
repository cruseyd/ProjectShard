using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _glowGold;
    [SerializeField] private ParticleSystem _glowBlue;

    public void Start()
    {
        Clear();
    }
    public void Clear()
    {
        MarkValidTarget(false);
        MarkSource(false);
        MarkActive(false);
    }
    public void MarkValidTarget(bool flag)
    {
        if (!_glowBlue.isPlaying && flag)
        {
            _glowBlue.Play();
        }
        else if (!flag)
        {
            _glowBlue.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public void MarkSource(bool flag)
    {
        if (!_glowGold.isPlaying && flag)
        {
            _glowGold.Play();
        }
        else if (!flag)
        {
            _glowGold.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public void MarkActive(bool flag) { }
}
