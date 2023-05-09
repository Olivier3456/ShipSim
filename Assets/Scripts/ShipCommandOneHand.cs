using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ShipCommandOneHand : MonoBehaviour
{
    [SerializeField] private XRBaseController _controller;

    [SerializeField] private GameObject _zeroPointMarker;
    [SerializeField] private GameObject _shipMarker;
    [SerializeField] private Rigidbody _shipRigidbody;
    [SerializeField] private ThrustersManager _thrustersManager;
    [Space(10)]
    [SerializeField] Material _shipMarkerTranslationMaterial;
    [SerializeField] Material _shipMarkerRotationMaterial;
    [SerializeField] Material _shipMarkerTranslationAndRotationMaterial;

    private bool _enterInTranslationControlMode = true;
    private bool _enterInRotationControlMode = true;

    private Vector3 _translationForcesToApplyToTheShip;
    private Vector3 _rotationForcesToApplyToTheShip;

    private Vector3 _handInitialPosition;

    private Quaternion _handInitialRotation;

    private int _ActiveControlModes = 0;

    private Renderer[] _shipMarkerChildrenRenderers;

    [Space(10)]
    [SerializeField] private float _translationsSensibility = 5.0f;
    [SerializeField] private float _forwardFactor = 2.0f;
    [SerializeField] private float _backwardFactor = 2.0f;
    [SerializeField] private float _rightFactor = 1.0f;
    [SerializeField] private float _leftFactor = 1.0f;
    [SerializeField] private float _upFactor = 1.0f;
    [SerializeField] private float _downFactor = 1.0f;
    [Space(10)]
    [SerializeField] private float _rotationsSensibility = 0.008f;
    [SerializeField] private float _rotationXFactor = 1.0f;
    [SerializeField] private float _rotationYFactor = 1.0f;
    [SerializeField] private float _rotationZFactor = 0.25f;
    [Space(10)]
    [SerializeField] private float _maxTranslationInputValue = 0.4f;

    private float _forwardThrusterValue;
    private float _backwardThrusterValue;
    private float _upThrusterValue;
    private float _downThrusterValue;
    private float _rightThrusterValue;
    private float _leftThrusterValue;


    private void Start()
    {
        _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition;
        _shipMarker.transform.localRotation = _zeroPointMarker.transform.localRotation;
        _shipMarkerChildrenRenderers = _shipMarker.GetComponentsInChildren<Renderer>();
    }


    private void Update()
    {
        Translation();
        Rotation();        
    }


    private void Translation()
    {
        if (_controller.activateInteractionState.value > 0.5f)
        {
            if (_enterInTranslationControlMode)
            {
                _ActiveControlModes++;
                InitialiseTranslation();
            }

            _translationForcesToApplyToTheShip = _controller.transform.localPosition - _handInitialPosition;


            // Limits the translation input values inside a sphere:
            float distance = _translationForcesToApplyToTheShip.magnitude;
            if (distance >= _maxTranslationInputValue)
            {
                _translationForcesToApplyToTheShip *= _maxTranslationInputValue / distance;
            }

            _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition + _translationForcesToApplyToTheShip;


            _backwardThrusterValue = -Mathf.Clamp(_translationForcesToApplyToTheShip.z, -Mathf.Infinity, 0);
            _thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThruster, false, _backwardThrusterValue * (1 / _maxTranslationInputValue));

            _forwardThrusterValue = Mathf.Clamp(_translationForcesToApplyToTheShip.z, 0, Mathf.Infinity);
            _thrustersManager.ChangeThrusterValues(_thrustersManager.ForwardThruster, false, _forwardThrusterValue * (1 / _maxTranslationInputValue));

            _rightThrusterValue = -Mathf.Clamp(_translationForcesToApplyToTheShip.x, -Mathf.Infinity, 0);
            _thrustersManager.ChangeThrusterValues(_thrustersManager.RightThruster, false, _rightThrusterValue * (1 / _maxTranslationInputValue));



            _translationForcesToApplyToTheShip.x = _translationForcesToApplyToTheShip.x > 0 ? _translationForcesToApplyToTheShip.x *= _rightFactor : _translationForcesToApplyToTheShip.x *= _leftFactor;
            _translationForcesToApplyToTheShip.y = _translationForcesToApplyToTheShip.y > 0 ? _translationForcesToApplyToTheShip.y *= _upFactor : _translationForcesToApplyToTheShip.y *= _downFactor;
            _translationForcesToApplyToTheShip.z = _translationForcesToApplyToTheShip.z > 0 ? _translationForcesToApplyToTheShip.z *= _forwardFactor : _translationForcesToApplyToTheShip.z *= _backwardFactor;

            ShipTranslation();
        }
        else if (!_enterInTranslationControlMode)
        {
            _ActiveControlModes--;
            ChangeShipMarkerDisplay(_shipMarkerRotationMaterial);
            
            _thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThruster, true);
            _thrustersManager.ChangeThrusterValues(_thrustersManager.ForwardThruster, true);
            _thrustersManager.ChangeThrusterValues(_thrustersManager.RightThruster, true);

            // _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition;
            StartCoroutine(LerpShipMarkerPositionToZeroPoint());

            _enterInTranslationControlMode = true;            
        }
    }
    
    
    private void Rotation()
    {
        if (_controller.selectInteractionState.value > 0.5f)
        {
            if (_enterInRotationControlMode)
            {
                _ActiveControlModes++;
                InitialiseRotation();
            }

            _shipMarker.transform.localRotation = Quaternion.Inverse(_handInitialRotation) * _controller.transform.localRotation;
            _rotationForcesToApplyToTheShip = _shipMarker.transform.localRotation.eulerAngles;

            CorrectAngle(ref _rotationForcesToApplyToTheShip.x, _rotationXFactor);
            CorrectAngle(ref _rotationForcesToApplyToTheShip.y, _rotationYFactor);
            CorrectAngle(ref _rotationForcesToApplyToTheShip.z, _rotationZFactor);
            ShipRotation();
        }
        else if (!_enterInRotationControlMode)
        {
            _ActiveControlModes--;
            ChangeShipMarkerDisplay(_shipMarkerTranslationMaterial);

            //_shipMarker.transform.localRotation = _zeroPointMarker.transform.localRotation;
            StartCoroutine(LerpShipMarkerRotationToZeroPoint());

            _enterInRotationControlMode = true;
        }
    }

    

    private void ShipTranslation()
    {
        _shipRigidbody.AddRelativeForce(_translationForcesToApplyToTheShip * _translationsSensibility);
    }

    private void ShipRotation()
    {
        _shipRigidbody.AddRelativeTorque(_rotationForcesToApplyToTheShip * _rotationsSensibility);
    }


    private void InitialiseTranslation()
    {
        _enterInTranslationControlMode = false;

        _handInitialPosition = _controller.transform.localPosition;

        ChangeShipMarkerDisplay(_shipMarkerTranslationMaterial);
    }

    private void InitialiseRotation()
    {
        _enterInRotationControlMode = false;

        _handInitialRotation = _controller.transform.localRotation;

        ChangeShipMarkerDisplay(_shipMarkerRotationMaterial);
    }


    private void ChangeShipMarkerDisplay(Material mat)
    {
        //if (_ActiveControlModes == 0) _shipMarker.SetActive(false);
        //else _shipMarker.SetActive(true);
        if (_ActiveControlModes > 0) _shipMarker.SetActive(true);


        if (_ActiveControlModes == 2)
        {
            for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
            {
                _shipMarkerChildrenRenderers[i].material = _shipMarkerTranslationAndRotationMaterial;
            }
        }
        else if (_ActiveControlModes == 1)
        {
            for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
            {
                _shipMarkerChildrenRenderers[i].material = mat;
            }
        }
    }

    IEnumerator LerpShipMarkerPositionToZeroPoint()
    {
        // float duration = 0.25f;
        // Vector3 initialPosition = _shipMarker.transform.localPosition;        

        while (Vector3.Distance(_shipMarker.transform.localPosition, _zeroPointMarker.transform.localPosition) >= 0.001f)
        {
            if (_controller.activateInteractionState.value > 0.5f) yield break;
            _shipMarker.transform.localPosition = Vector3.Lerp(_shipMarker.transform.localPosition, _zeroPointMarker.transform.localPosition, 0.2f);

            // Pour une vitesse constante :
            //    _shipMarker.transform.localPosition = Vector3.Lerp(initialPosition, _zeroPointMarker.transform.localPosition, time * (1 / duration));

            yield return null;
        }
        _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition;
        if (_ActiveControlModes == 0) _shipMarker.SetActive(false);
    }

    IEnumerator LerpShipMarkerRotationToZeroPoint()
    {
        while (Quaternion.Dot(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation) < 0.995f)
        {
            if (_controller.selectInteractionState.value > 0.5f) yield break;
            _shipMarker.transform.localRotation = Quaternion.Lerp(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation, 0.15f);
            yield return null;
        }
        _shipMarker.transform.localRotation = _zeroPointMarker.transform.localRotation;
        if (_ActiveControlModes == 0) _shipMarker.SetActive(false);
    }

    private float CorrectAngle(ref float angle, float factor)
    {
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;
        angle *= factor;
        return angle;
    }
}