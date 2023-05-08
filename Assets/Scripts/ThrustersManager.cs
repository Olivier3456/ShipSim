using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Thruster
{    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    private float value1;
    private float value2;

        
    public ParticleSystem ThrusterParticleSystem { get { return _particleSystem; } }
    public AudioSource ThrusterAudioSource { get { return _audioSource; } }
}

public class ThrustersManager : MonoBehaviour
{
    [SerializeField] private Thruster _backwardThruster;
    public Thruster BackWardThruster { get { return _backwardThruster; } }


    public void ChangeThrusterValues(Thruster thruster, float valueToAddToVolumeAudioSource)
    {
        thruster.ThrusterAudioSource.volume = valueToAddToVolumeAudioSource;
    }




    void Update()
    {
        
    }
}
