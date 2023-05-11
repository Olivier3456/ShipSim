using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Thruster
{
    [SerializeField] private Transform _transform;
    public Transform CenterPosition;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    public ParticleSystem ThrusterParticleSystem { get { return _particleSystem; } }
    public AudioSource ThrusterAudioSource { get { return _audioSource; } }
    public Transform ThrusterTransform { get { return _transform; } }     
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


    [SerializeField] private Thruster _upThruster;
    public Thruster UpThruster { get { return _upThruster; } }


    [SerializeField] private Thruster _downThruster;
    public Thruster DownThruster { get { return _downThruster; } }




    [SerializeField] private Thruster _rotationThruster;
    public Thruster RotationThruster { get { return _rotationThruster; } }





    public void ChangeThrusterValues(Thruster thruster, float value, float minimumValue = 0.01f)
    {
        if (value < minimumValue && thruster.ThrusterAudioSource.isPlaying)
        {
            thruster.ThrusterAudioSource.Stop();
        }
        else if (value > minimumValue && !thruster.ThrusterAudioSource.isPlaying)
        {
            thruster.ThrusterAudioSource.Play();
        }
        else if (value > minimumValue)
        {
            thruster.ThrusterAudioSource.volume = value;
            thruster.ThrusterAudioSource.pitch = 1 + (value * 0.5f);
        }
    }


    public void ChangeRotationThrusterValues(Thruster thruster, float value, Vector3 rotation)
    {
        ChangeThrusterValues(thruster, value);
        thruster.ThrusterTransform.localPosition = thruster.CenterPosition.localPosition + rotation.normalized;
    }



    //}public void ChangeThrusterValues(Thruster thruster, bool decreaseOverTimer, float value = 0)
    //{        
    //    if (!decreaseOverTimer)
    //    {
    //        thruster.IsActive = true;
    //        thruster.ThrusterAudioSource.volume = value;
    //        thruster.ThrusterAudioSource.pitch = 1 + (value * 0.5f);
    //    }
    //    else
    //    {
    //        thruster.IsActive = false;
    //        StartCoroutine(DecreaseThrusterValuesOverTime(thruster));
    //    }
    //}

    //IEnumerator DecreaseThrusterValuesOverTime(Thruster thruster)
    //{
    //    if (thruster.ThrusterAudioSource.volume >= 0.01f && !thruster.IsActive)
    //    {
    //        thruster.ThrusterAudioSource.volume -= Time.deltaTime;
    //        thruster.ThrusterAudioSource.pitch -= Time.deltaTime;
    //        yield return null;
    //        StartCoroutine(DecreaseThrusterValuesOverTime(thruster));
    //    }        
    //}
}
