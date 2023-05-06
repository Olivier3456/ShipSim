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
    [Space(10)]
    [SerializeField] Material _shipMarkerTranslationMaterial;
    [SerializeField] Material _shipMarkerRotationMaterial;
    [SerializeField] Material _shipMarkerTranslationAndRotationMaterial;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI _debugText1;
    [SerializeField] private TextMeshProUGUI _debugText2;


    private bool _enterInTranslationControlMode = true;
    private bool _enterInRotationControlMode = true;


    private Vector3 _translationForcesToApplyToTheShip;
    private Vector3 _rotationForcesToApplyToTheShip;

    private Vector3 _shipMarkerInitialPosition;
    private Quaternion _shipMarkerInitialRotation;

    private Vector3 _handInitialPosition;

    private Quaternion _handInitialRotation;

    private int _handsInControl = 0;

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
    [SerializeField] private float _maxShipMarkerTranslation = 0.45f;


    private void Start()
    {
        _shipMarkerInitialPosition = _shipMarker.transform.localPosition;
        _shipMarkerInitialRotation = _shipMarker.transform.localRotation;

        _shipMarkerChildrenRenderers = _shipMarker.GetComponentsInChildren<Renderer>();
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
                _handsInControl++;
                InitialiseTranslation();
            }

            _translationForcesToApplyToTheShip = _controller.transform.localPosition - _handInitialPosition;


            //Mathf.Clamp(_translationForcesToApplyToTheShip.x, -_maxShipMarkerTranslation, _maxShipMarkerTranslation);
            //Mathf.Clamp(_translationForcesToApplyToTheShip.y, -_maxShipMarkerTranslation, _maxShipMarkerTranslation);
            //Mathf.Clamp(_translationForcesToApplyToTheShip.z, -_maxShipMarkerTranslation, _maxShipMarkerTranslation);


            // Limits the shipmarker translations inside a sphere:
            float distance = _translationForcesToApplyToTheShip.magnitude;
            if (distance >= _maxShipMarkerTranslation)
            {
                _translationForcesToApplyToTheShip *= _maxShipMarkerTranslation / distance;
            }           
            _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition + _translationForcesToApplyToTheShip;


            _translationForcesToApplyToTheShip.x = _translationForcesToApplyToTheShip.x > 0 ? _translationForcesToApplyToTheShip.x *= _rightFactor : _translationForcesToApplyToTheShip.x *= _leftFactor;
            _translationForcesToApplyToTheShip.y = _translationForcesToApplyToTheShip.y > 0 ? _translationForcesToApplyToTheShip.y *= _upFactor : _translationForcesToApplyToTheShip.y *= _downFactor;
            _translationForcesToApplyToTheShip.z = _translationForcesToApplyToTheShip.z > 0 ? _translationForcesToApplyToTheShip.z *= _forwardFactor : _translationForcesToApplyToTheShip.z *= _backwardFactor;

            ShipTranslation();
        }
        else if (!_enterInTranslationControlMode)
        {
            _handsInControl--;
            ChangeShipMarkerDisplay(_shipMarkerRotationMaterial);

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
                _handsInControl++;
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
            _handsInControl--;
            ChangeShipMarkerDisplay(_shipMarkerTranslationMaterial);

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
        if (_handsInControl == 0) _shipMarker.SetActive(false);
        else _shipMarker.SetActive(true);

        if (_handsInControl == 2)
        {
            for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
            {
                _shipMarkerChildrenRenderers[i].material = _shipMarkerTranslationAndRotationMaterial;
            }
        }
        else if (_handsInControl == 1)
        {
            for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
            {
                _shipMarkerChildrenRenderers[i].material = mat;
            }
        }
    }
}