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
    [SerializeField] ParticleSystem _burst;

    void Start()
    {
        Play(_goldShimmer, false);
        Play(_blueShimmer, false);
        Play(_redShimmer, false);
        Play(_goldGlow, false);
        Play(_redGlow, false);
        Play(_burst, false);
    }

    public void Clear()
    {
        Play(_goldShimmer, false);
        Play(_blueShimmer, false);
        Play(_redShimmer, false);
        Play(_goldGlow, false);
        Play(_redGlow, false);
    }

    public void ClearGlow()
    {
        Play(_goldGlow, false);
        Play(_redGlow, false);
    }

    public void ClearShimmer()
    {
        Play(_goldShimmer, false);
        Play(_blueShimmer, false);
        Play(_redShimmer, false);
    }

    public void ShimmerBlue()
    {
        Play(_redShimmer, false);
        Play(_blueShimmer, true);
        Play(_goldShimmer, false);
    }
    public void GlowGold()
    {
        Play(_goldGlow, true);
        Play(_redGlow, false);
    }
    public void ShimmerGold()
    {
        Play(_redShimmer, false);
        Play(_blueShimmer, false);
        Play(_goldShimmer, true);
    }
    public void ShimmerRed()
    {
        Play(_redShimmer, true);
        Play(_blueShimmer, false);
        Play(_goldShimmer, false);
    }

    public void GlowRed()
    {
        Play(_redGlow, true);
        Play(_goldGlow, false);
    }

    public void Burst()
    {
        _burst.Stop();
        _burst.Play();
    }

    private void Play(ParticleSystem system, bool flag)
    {
        ParticleSystem.EmissionModule emission = system.emission;
        if ((emission.enabled && !flag) || (!emission.enabled && flag))
        {
            system.gameObject.SetActive(flag);
            emission.enabled = flag;
        }
    }
}
