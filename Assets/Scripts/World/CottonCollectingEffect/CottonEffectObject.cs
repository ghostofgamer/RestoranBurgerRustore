using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonEffectObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    public void Play()
    {
        gameObject.SetActive(true);
        particle.Play();
    }

    public void Stop()
    {
        gameObject.SetActive(false);
        particle.Stop();
    }
}