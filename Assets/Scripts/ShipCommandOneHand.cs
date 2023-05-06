using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ShipCommandOneHand : MonoBehaviour
{
    [SerializeField] private XRBaseController _controller;
    [SerializeField] private GameObject _hand;

    [SerializeField] private GameObject _zeroPointMarker;

    [SerializeField] private GameObject _shipMarker;

    [SerializeField] private Rigidbody _shipRigidbody;

    [SerializeField] private float _translationsSensibility = 1.0f;
    

    [SerializeField] private TextMeshProUGUI _debugText1;
    [SerializeField] private TextMeshProUGUI _debugText2;


    private bool _enterInTranslationControlMode = true;
    private bool _enterInRotationControlMode = true;


    private Vector3 _translationForcesToApplyToTheShip;
    private Vector3 _rotationForceToApplyToTheShip;

    private Vector3 _shipMarkerInitialPosition;
    private Quaternion _shipMarkerInitialRotation;
    
    private Vector3 _handInitialPosition;

    private Quaternion _handInitialRotation;

    private int _handsInControl = 0;

    [Space(10)]
    [SerializeField] private float _rotationsSensibility = 0.0025f;
    [SerializeField] private float _rotationXFactor = 1;
    [SerializeField] private float _rotationYFactor = 1;
    [SerializeField] private float _rotationZFactor = 1;
       

    private void Start()
    {
        _shipMarkerInitialPosition = _shipMarker.transform.localPosition;
        _shipMarkerInitialRotation = _shipMarker.transform.localRotation;
    }


    private void Update()
    {
        Translation();        
        Rotation();

        // Pour debugger :
        // if (_translationController.activateInteractionState.value > 0.5f || _rotationController.activateInteractionState.value > 0.5f) SceneManager.LoadSceneAsync(0);
    }
       

    private void Translation()
    {
        if (_controller.activateInteractionState.value > 0.5f)
        {
            if (_enterInTranslationControlMode)
            {
                InitialiseTranslation();
                _handsInControl++;
            }

            _translationForcesToApplyToTheShip = _controller.transform.localPosition - _handInitialPosition;
            _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition + _translationForcesToApplyToTheShip;
            ShipTranslation();


        }
        else if (!_enterInTranslationControlMode)
        {
            _handsInControl--;
            if (_handsInControl == 0) _shipMarker.SetActive(false);

            _shipMarker.transform.localPosition = _shipMarkerInitialPosition;

            _enterInTranslationControlMode = true;
        }
    }

    private void Rotation()
    {
        if (_controller.selectInteractionState.value > 0.5f)
        {
            if (_enterInRotationControlMode)
            {
                InitialiseRotation();
                _handsInControl++;
            }

            _shipMarker.transform.localRotation = Quaternion.Inverse(_handInitialRotation) * _controller.transform.localRotation;
            _rotationForceToApplyToTheShip = _shipMarker.transform.localRotation.eulerAngles;


           // _debugText1.text = _rotationForceToApplyToTheShip.z.ToString();

            CorrectAngle(ref _rotationForceToApplyToTheShip.x, _rotationXFactor);
            CorrectAngle(ref _rotationForceToApplyToTheShip.y, _rotationYFactor);
            CorrectAngle(ref _rotationForceToApplyToTheShip.z, _rotationZFactor);


            //_debugText2.text = _rotationForceToApplyToTheShip.z.ToString();

            ShipRotation();


        }
        else if (!_enterInRotationControlMode)
        {
            _handsInControl--;
            if (_handsInControl == 0) _shipMarker.SetActive(false);

            _shipMarker.transform.localRotation = _shipMarkerInitialRotation;

            _enterInRotationControlMode = true;
        }
    }

    private float CorrectAngle(ref float angle, float factor)
    {
        
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;

        angle *= factor;


        return angle;
    }


    private void ShipTranslation()
    {
        _shipRigidbody.AddRelativeForce(_translationForcesToApplyToTheShip * _translationsSensibility);
    }

    private void ShipRotation()
    {
        _shipRigidbody.AddRelativeTorque(_rotationForceToApplyToTheShip * _rotationsSensibility);
    }


    private void InitialiseTranslation()
    {
        _enterInTranslationControlMode = false;

        _shipMarker.SetActive(true);

        _handInitialPosition = _controller.transform.localPosition;
    }

    private void InitialiseRotation()
    {
        _enterInRotationControlMode = false;

        _shipMarker.SetActive(true);       

        _handInitialRotation = _controller.transform.localRotation;
    }
}