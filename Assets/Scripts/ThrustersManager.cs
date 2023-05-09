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
    [HideInInspector] public bool IsActive;
        
    public ParticleSystem ThrusterParticleSystem { get { return _particleSystem; } }
    public AudioSource ThrusterAudioSource { get { return _audioSource; } } 
    
}

public class ThrustersManager : MonoBehaviour
{
    [SerializeField] private Thruster _backwardThruster;
    public Thruster BackwardThruster { get { return _backwardThruster; } }

    [SerializeField] private Thruster _forwardThruster;
    public Thruster ForwardThruster { get { return _forwardThruster; } }

    [SerializeField] private Thruster _rightThruster;
    public Thruster RightThruster { get { return _rightThruster; } }

    [SerializeField] private Thruster _leftThruster;
    public Thruster LeftThruster { get { return _leftThruster; } }


    public void ChangeThrusterValues(Thruster thruster, bool decreaseOverTimer, float value = 0)
    {        
        if (!decreaseOverTimer)
        {
            thruster.IsActive = true;
            thruster.ThrusterAudioSource.volume = value;
            thruster.ThrusterAudioSource.pitch = 1 + (value * 0.5f);
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
            thruster.ThrusterAudioSource.pitch -= Time.deltaTime;
            yield return null;
            StartCoroutine(DecreaseThrusterValuesOverTime(thruster));
        }        
    }
}
