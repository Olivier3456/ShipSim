using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Thruster
{    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    private float value1;
    private float value2;
    public bool IsActive;
        
    public ParticleSystem ThrusterParticleSystem { get { return _particleSystem; } }
    public AudioSource ThrusterAudioSource { get { return _audioSource; } } 
    
}

public class ThrustersManager : MonoBehaviour
{
    [SerializeField] private Thruster _backwardThruster;
    public Thruster BackWardThruster { get { return _backwardThruster; } }


    public void ChangeThrusterValues(Thruster thruster, bool decreaseOverTimer, float audioSourceVolume = 0)
    {        
        if (!decreaseOverTimer)
        {
            thruster.IsActive = true;
            thruster.ThrusterAudioSource.volume = audioSourceVolume;
        }
        else
        {
            thruster.IsActive = false;
            StartCoroutine(DecreaseThrusterValuesOverTime(thruster));
        }
    }

    IEnumerator DecreaseThrusterValuesOverTime(Thruster thruster)
    {
        if (thruster.ThrusterAudioSource.volume >= 0.01f && !thruster.IsActive)
        {
            thruster.ThrusterAudioSource.volume -= Time.deltaTime;
            yield return null;
            StartCoroutine(DecreaseThrusterValuesOverTime(thruster));
        }        
    }
}
