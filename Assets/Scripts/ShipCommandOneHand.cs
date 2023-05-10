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

    private Vector3 _translationValue;

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
    private float _rightThrusterValue;
    private float _leftThrusterValue;
    private float _upThrusterValue;
    private float _downThrusterValue;



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

            _translationValue = _controller.transform.localPosition - _handInitialPosition;


            // Limits the translation input values inside a sphere:
            _translationValue = LimitTranslationValue(_translationValue);

            _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition + _translationValue;

            CalculateAndSendThrustersValuesToThrustersManager(_translationValue);

            _translationForcesToApplyToTheShip.x = _translationValue.x > 0 ? _translationValue.x *= _rightFactor : _translationValue.x *= _leftFactor;
            _translationForcesToApplyToTheShip.y = _translationValue.y > 0 ? _translationValue.y *= _upFactor : _translationValue.y *= _downFactor;
            _translationForcesToApplyToTheShip.z = _translationValue.z > 0 ? _translationValue.z *= _forwardFactor : _translationValue.z *= _backwardFactor;

            ShipTranslation();
        }
        else if (!_enterInTranslationControlMode)
        {
            _ActiveControlModes--;
            ChangeShipMarkerDisplay(_shipMarkerRotationMaterial);

            // Peut-être simplement faire ces appels dans les méthodes Lerp ci-dessous. Donc supprimer le paramètre true et corriger la fonction dans ThrustersManager.
            //_thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThruster, true);
            //_thrustersManager.ChangeThrusterValues(_thrustersManager.ForwardThruster, true);
            //_thrustersManager.ChangeThrusterValues(_thrustersManager.RightThruster, true);

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

            Vector3 translation = _shipMarker.transform.localPosition - _zeroPointMarker.transform.localPosition;
            CalculateAndSendThrustersValuesToThrustersManager(translation);

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

    private Vector3 LimitTranslationValue(Vector3 translation)
    {
        float distance = translation.magnitude;
        if (distance >= _maxTranslationInputValue)
        {
            translation *= _maxTranslationInputValue / distance;
        }
        return translation;
    }


    

    private void CalculateAndSendThrustersValuesToThrustersManager(Vector3 translation)
    {
        _backwardThrusterValue = -Mathf.Clamp(translation.z, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThruster, _backwardThrusterValue * (1 / _maxTranslationInputValue));

        _forwardThrusterValue = Mathf.Clamp(translation.z, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.ForwardThruster, _forwardThrusterValue * (1 / _maxTranslationInputValue));

        _rightThrusterValue = -Mathf.Clamp(translation.x, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.RightThruster, _rightThrusterValue * (1 / _maxTranslationInputValue));

        _leftThrusterValue = Mathf.Clamp(translation.x, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.LeftThruster, _leftThrusterValue * (1 / _maxTranslationInputValue));

        _upThrusterValue = -Mathf.Clamp(translation.y, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.UpThruster, _upThrusterValue * (1 / _maxTranslationInputValue));

        _downThrusterValue = Mathf.Clamp(translation.y, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.DownThruster, _downThrusterValue * (1 / _maxTranslationInputValue));        
    }



    //private void CalculateAndSendThrustersValuesToThrustersManager(Vector3 translation)
    //{
    //    // Forward / Backward thrusters:
    //    if (translation.z > 0.01f || translation.z < -0.01f)
    //        _thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThrusters, translation.z * (1 / _maxTranslationInputValue));
    //}
}