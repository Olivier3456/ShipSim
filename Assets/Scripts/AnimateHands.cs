using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{

    [SerializeField] private InputActionProperty _pinchAction;

    [SerializeField] private InputActionProperty _gripAction;
    
    [SerializeField] private Animator _animator;
    
    void Update()
    {
        float gripVal = _gripAction.action.ReadValue<float>();
        _animator.SetFloat("Pointing",gripVal);




        //float triggerVal = _pinchAction.action.ReadValue<float>();
        //_animator.SetFloat("Trigger", triggerVal);

        //float gripVal = _gripAction.action.ReadValue<float>();
        //_animator.SetFloat("Grip",gripVal);
    }
}