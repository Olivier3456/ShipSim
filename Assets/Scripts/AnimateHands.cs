using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _animationSpeed = 4;

    private bool _handInTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand trigger"))
        {
            _handInTrigger = true;
            StartCoroutine(HandEnteredTrigger());
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand trigger"))
        {
            _handInTrigger = false;
            StartCoroutine(HandExitedTrigger());
        }
    }

    IEnumerator HandExitedTrigger()
    {
        while (_animator.GetFloat("Pointing") > 0.001f)
        {
            if (_handInTrigger) { yield break; }
            _animator.SetFloat("Pointing", _animator.GetFloat("Pointing") - Time.deltaTime * _animationSpeed);
            yield return null;
        }

        // Debug.Log("Sortie de la coroutine HandExitedTrigger");
    }

    IEnumerator HandEnteredTrigger()
    {
        while (_animator.GetFloat("Pointing") < 0.999f)
        {
            if (!_handInTrigger) { yield break; }
            _animator.SetFloat("Pointing", _animator.GetFloat("Pointing") + Time.deltaTime * _animationSpeed);
            yield return null;
        }

        // Debug.Log("Sortie de la coroutine HandEnteredTrigger");
    }



}




//public class AnimateHands : MonoBehaviour
//{

//    [SerializeField] private InputActionProperty _pinchAction;

//    [SerializeField] private InputActionProperty _gripAction;

//    [SerializeField] private Animator _animator;

//    void Update()
//    {
//        float gripVal = _gripAction.action.ReadValue<float>();
//        _animator.SetFloat("Pointing",gripVal);




//        //float triggerVal = _pinchAction.action.ReadValue<float>();
//        //_animator.SetFloat("Trigger", triggerVal);

//        //float gripVal = _gripAction.action.ReadValue<float>();
//        //_animator.SetFloat("Grip",gripVal);
//    }
//}