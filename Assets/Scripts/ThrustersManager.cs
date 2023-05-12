using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Thruster
{
    [Tooltip("Only necessary for the rotations thruster")]
    [SerializeField] private Transform _transform;    
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource[] _audioSources;

    public ParticleSystem ThrusterParticleSystem { get { return _particleSystem; } }
    public AudioSource[] ThrusterAudioSource { get { return _audioSources; } }
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
        for (int i = 0; i < thruster.ThrusterAudioSource.Length; i++)
        {
            if (value < minimumValue && thruster.ThrusterAudioSource[i].isPlaying)
            {
                thruster.ThrusterAudioSource[i].Stop();
            }
            else if (value > minimumValue && !thruster.ThrusterAudioSource[i].isPlaying)
            {
                thruster.ThrusterAudioSource[i].Play();
            }
            else if (value > minimumValue)
            {
                thruster.ThrusterAudioSource[i].volume = value;
                thruster.ThrusterAudioSource[i].pitch = 1 + (value * 0.5f);
            }
        }       
    }


    public void ChangeRotationThrusterValues(Thruster thruster, float value, Quaternion rotation)
    {
        ChangeThrusterValues(thruster, value);
        thruster.ThrusterTransform.localRotation = Quaternion.Inverse(rotation);
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
