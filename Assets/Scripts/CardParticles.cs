using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardParticles : MonoBehaviour
{

    [SerializeField] ParticleSystem _goldShimmer;
    [SerializeField] ParticleSystem _blueShimmer;
    [SerializeField] ParticleSystem _glow;
    void Start()
    {
        _goldShimmer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _blueShimmer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _glow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Clear()
    {
        Play(_goldShimmer, false);
        Play(_blueShimmer, false);
        Play(_glow, false);
    }

    public void MarkValidTarget()
    {
        Play(_blueShimmer, true);
        Play(_goldShimmer, false);
    }
    public void MarkSource()
    {
        Play(_blueShimmer, false);
        Play(_goldShimmer, true);
    }
    public void MarkActive()
    {
        Play(_blueShimmer, false);
        Play(_goldShimmer, true);
    }

    public void Glow(bool flag)
    {
        Play(_glow, flag);
    }

    private void Play(ParticleSystem system, bool flag)
    {
        if (!system.gameObject.activeSelf) { system.gameObject.SetActive(true); }
        if (!system.isPlaying && flag)
        {
            system.Play();
        }
        else if (!flag)
        {
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
