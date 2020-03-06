using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardParticles : MonoBehaviour
{

    [SerializeField] ParticleSystem _goldShimmer;
    [SerializeField] ParticleSystem _blueShimmer;
    [SerializeField] ParticleSystem _redShimmer;
    [SerializeField] ParticleSystem _goldGlow;
    [SerializeField] ParticleSystem _redGlow;

    void Start()
    {
        _goldShimmer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _blueShimmer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _redShimmer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _goldGlow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _redGlow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Clear()
    {
        Play(_goldShimmer, false);
        Play(_blueShimmer, false);
        Play(_redShimmer, false);
        Play(_goldGlow, false);
        Play(_redGlow, false);
    }

    public void MarkValidTarget()
    {
        Play(_goldGlow, false);
        Play(_redGlow, false);
        Play(_redShimmer, false);
        Play(_blueShimmer, true);
        Play(_goldShimmer, false);
    }
    public void MarkSource()
    {
        Play(_goldGlow, true);
        Play(_redGlow, false);
        Play(_redShimmer, false);
        Play(_blueShimmer, false);
        Play(_goldShimmer, false);
    }
    public void MarkActive()
    {
        Play(_goldGlow, false);
        Play(_redGlow, false);
        Play(_redShimmer, false);
        Play(_blueShimmer, false);
        Play(_goldShimmer, true);
    }
    public void MarkNeedsUpkeep()
    {
        Play(_goldGlow, false);
        Play(_redGlow, false);
        Play(_redShimmer, true);
        Play(_blueShimmer, false);
        Play(_goldShimmer, false);
    }

    public void Glow(bool flag)
    {
        Play(_goldGlow, flag);
    }

    public void RedGlow(bool flag)
    {
        Play(_redGlow, flag);
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
