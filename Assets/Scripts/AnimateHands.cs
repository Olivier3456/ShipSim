using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{

    //[SerializeField] private InputActionProperty _pinchAction;

    //[SerializeField] private InputActionProperty _gripAction;

    [SerializeField] private Animator _animator;

    //void Update()
    //{
    //    float gripVal = _gripAction.action.ReadValue<float>();
    //    _animator.SetFloat("Pointing", gripVal);

    //}



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand trigger"))
        _animator.SetFloat("Pointing", 1);
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand trigger"))
            _animator.SetFloat("Pointing", 0);
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