using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ShipCommandOneHand : MonoBehaviour
{
    [SerializeField] private XRBaseController _rightController;
    [SerializeField] private XRBaseController _leftController;

    private XRBaseController _rotationController;
    private XRBaseController _translationController;

    [SerializeField] private GameObject _zeroPointMarker;
    [SerializeField] private GameObject _shipMarker;
    [SerializeField] private Rigidbody _shipRigidbody;
    [SerializeField] private ThrustersManager _thrustersManager;
    [Space(10)]
    [SerializeField] private Material _shipMarkerTranslationMaterial;
    [SerializeField] private Material _shipMarkerRotationMaterial;
    [SerializeField] private Material _shipMarkerTranslationAndRotationMaterial;
    [Space(10)]
    [SerializeField] private InputActionReference _inputActionsRotationLeftHand;
    [SerializeField] private InputActionReference _inputActionsRotationRightHand;
    [SerializeField] private InputActionReference _inputActionsTranslationLeftHand;
    [SerializeField] private InputActionReference _inputActionsTranslationRightHand;

    private bool _enterInTranslationControlMode = true;
    private bool _enterInRotationControlMode = true;

    private Vector3 _translationForcesToApplyToTheShip;
    private Vector3 _rotationForcesToApplyToTheShip;

    private Vector3 _handInitialPosition;

    private Quaternion _handInitialRotation;

    private Renderer[] _shipMarkerChildrenRenderers;

    [Space(10)]
    [SerializeField] private float _translationsSensibility = 5.0f;
    [SerializeField][Range(0, 1)] private float _forwardFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _backwardFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _rightFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _leftFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _upFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _downFactor = 1.0f;
    [Space(10)]
    [SerializeField] private float _maxTranslationInputValue = 0.2f;
    [Space(20)]
    [SerializeField] private float _rotationsSensibility = 0.0001f;
    [SerializeField][Range(0, 1)] private float _rotationXFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _rotationYFactor = 1.0f;
    [SerializeField][Range(0, 1)] private float _rotationZFactor = 1.0f;


    public TextMeshProUGUI DebugText;


    private void Start()
    {
        _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition;
        _shipMarker.transform.localRotation = _zeroPointMarker.transform.localRotation;
        _shipMarkerChildrenRenderers = _shipMarker.GetComponentsInChildren<Renderer>();

        _shipRigidbody.centerOfMass = _shipRigidbody.transform.position;


        _inputActionsRotationLeftHand.action.Enable();
        _inputActionsRotationLeftHand.action.performed += RotationInputActionLeftHand;
        _inputActionsRotationRightHand.action.Enable();
        _inputActionsRotationRightHand.action.performed += RotationInputActionRightHand;

        _inputActionsTranslationLeftHand.action.Enable();
        _inputActionsTranslationLeftHand.action.performed += TranslationInputActionLeftHand;
        _inputActionsTranslationRightHand.action.Enable();
        _inputActionsTranslationRightHand.action.performed += TranslationInputActionRightHand;
    }

    private void FixedUpdate()
    {
        Translation();
        Rotation();
    }

    private void RotationInputActionLeftHand(InputAction.CallbackContext obj)     // Interaction modes for the buttons must be set to "Press > Press and Release".
    {
        float inputActionValue = _inputActionsRotationLeftHand.action.ReadValue<float>();    // Action Type must be set to "Value".
        SetRotationOrTranslationController(inputActionValue, _leftController, ref _rotationController);
    }

    private void RotationInputActionRightHand(InputAction.CallbackContext obj)
    {
        float inputActionValue = _inputActionsRotationRightHand.action.ReadValue<float>();
        SetRotationOrTranslationController(inputActionValue, _rightController, ref _rotationController);
    }

    private void TranslationInputActionLeftHand(InputAction.CallbackContext obj)
    {
        float inputActionValue = _inputActionsTranslationLeftHand.action.ReadValue<float>();
        SetRotationOrTranslationController(inputActionValue, _leftController, ref _translationController);
    }

    private void TranslationInputActionRightHand(InputAction.CallbackContext obj)
    {
        float inputActionValue = _inputActionsTranslationRightHand.action.ReadValue<float>();
        SetRotationOrTranslationController(inputActionValue, _rightController, ref _translationController);
    }

    private void SetRotationOrTranslationController(float inputActionValue, XRBaseController leftOrRightController, ref XRBaseController rotationOrTranslationController)
    {
        if (inputActionValue == 1 && rotationOrTranslationController == null)
        {
            rotationOrTranslationController = leftOrRightController;
        }
        else if (rotationOrTranslationController == leftOrRightController)
        {
            rotationOrTranslationController = null;
        }
    }





    private void Translation()
    {
        if (_translationController != null)
        {
            if (_enterInTranslationControlMode)
            {
                InitialiseTranslation();
            }

            _translationForcesToApplyToTheShip = _translationController.transform.localPosition - _handInitialPosition;

            _translationForcesToApplyToTheShip = LimitTranslationValue(_translationForcesToApplyToTheShip);

            _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition + _translationForcesToApplyToTheShip;

            _translationForcesToApplyToTheShip *= 1 / _maxTranslationInputValue;

            CalculateAndSendThrustersTranslationValuesToThrustersManager(_translationForcesToApplyToTheShip);

            _translationForcesToApplyToTheShip.x = _translationForcesToApplyToTheShip.x > 0 ? _translationForcesToApplyToTheShip.x *= _rightFactor : _translationForcesToApplyToTheShip.x *= _leftFactor;
            _translationForcesToApplyToTheShip.y = _translationForcesToApplyToTheShip.y > 0 ? _translationForcesToApplyToTheShip.y *= _upFactor : _translationForcesToApplyToTheShip.y *= _downFactor;
            _translationForcesToApplyToTheShip.z = _translationForcesToApplyToTheShip.z > 0 ? _translationForcesToApplyToTheShip.z *= _forwardFactor : _translationForcesToApplyToTheShip.z *= _backwardFactor;

            ShipTranslation();
        }
        else if (!_enterInTranslationControlMode)
        {
            StartCoroutine(LerpShipMarkerPositionToZeroPoint());

            _enterInTranslationControlMode = true;
        }
    }


    private void Rotation()
    {
        if (_rotationController != null)
        {
            if (_enterInRotationControlMode)
            {
                InitialiseRotation();
            }

            _shipMarker.transform.localRotation = Quaternion.Inverse(_handInitialRotation) * _rotationController.transform.localRotation;

            _rotationForcesToApplyToTheShip = _shipMarker.transform.localRotation.eulerAngles;

            CorrectAngle(ref _rotationForcesToApplyToTheShip.x);
            CorrectAngle(ref _rotationForcesToApplyToTheShip.y);
            CorrectAngle(ref _rotationForcesToApplyToTheShip.z);

            CalculateAndSendThrustersRotationValuesToThrustersManager(_rotationForcesToApplyToTheShip, _shipMarker.transform.localRotation);

            ApplyRotationFactor(ref _rotationForcesToApplyToTheShip.x, _rotationXFactor);
            ApplyRotationFactor(ref _rotationForcesToApplyToTheShip.y, _rotationYFactor);
            ApplyRotationFactor(ref _rotationForcesToApplyToTheShip.z, _rotationZFactor);

            ShipRotation();
        }
        else if (!_enterInRotationControlMode)
        {
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




    IEnumerator LerpShipMarkerPositionToZeroPoint()
    {
        while (Vector3.Distance(_shipMarker.transform.localPosition, _zeroPointMarker.transform.localPosition) >= 0.001f)
        {
            if (_translationController != null) yield break;
            _shipMarker.transform.localPosition = Vector3.Lerp(_shipMarker.transform.localPosition, _zeroPointMarker.transform.localPosition, 0.2f);

            Vector3 translation = _shipMarker.transform.localPosition - _zeroPointMarker.transform.localPosition;
            CalculateAndSendThrustersTranslationValuesToThrustersManager(translation);

            yield return null;
        }
        _shipMarker.transform.localPosition = _zeroPointMarker.transform.localPosition;
        ChangeShipMarkerDisplay();
    }

    IEnumerator LerpShipMarkerRotationToZeroPoint()
    {
        DebugText.text = "Enter in LerpRotation";


        while (Quaternion.Dot(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation) < 0.9999f
            && Quaternion.Dot(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation) > -0.9999f)
        {
            if (_rotationController != null) yield break;
            _shipMarker.transform.localRotation = Quaternion.Lerp(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation, 0.15f);

            Vector3 rotation = _shipMarker.transform.localRotation.eulerAngles;
            CorrectAngle(ref rotation.x);
            CorrectAngle(ref rotation.y);
            CorrectAngle(ref rotation.z);
            CalculateAndSendThrustersRotationValuesToThrustersManager(rotation, _shipMarker.transform.localRotation);

            yield return null;

            DebugText.text = Quaternion.Dot(_shipMarker.transform.localRotation, _zeroPointMarker.transform.localRotation).ToString();
        }

        CalculateAndSendThrustersRotationValuesToThrustersManager(Vector3.zero, Quaternion.identity);
        _shipMarker.transform.localRotation = _zeroPointMarker.transform.localRotation;
        ChangeShipMarkerDisplay();

        DebugText.text = "LerpRotation is Finished";
    }

    private float CorrectAngle(ref float angle)
    {
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;
        return angle;
    }


    private float ApplyRotationFactor(ref float angle, float factor)
    {
        return angle *= factor;
    }



    private void CalculateAndSendThrustersRotationValuesToThrustersManager(Vector3 vector3Rotation, Quaternion quaternionRotation)
    {
        Vector3 absRotation = vector3Rotation;
        absRotation.x = Mathf.Abs(absRotation.x);
        absRotation.y = Mathf.Abs(absRotation.y);
        absRotation.z = Mathf.Abs(absRotation.z);
        float rotationThrusterValue = Mathf.Max(absRotation.x, absRotation.y, absRotation.z) / 180;


        _thrustersManager.ChangeRotationThrusterValues(_thrustersManager.RotationThruster, rotationThrusterValue, quaternionRotation);
    }



    private void CalculateAndSendThrustersTranslationValuesToThrustersManager(Vector3 translation)
    {
        float _backwardThrusterValue = -Mathf.Clamp(translation.z, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.BackwardThruster, _backwardThrusterValue);

        float _forwardThrusterValue = Mathf.Clamp(translation.z, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.ForwardThruster, _forwardThrusterValue);

        float _rightThrusterValue = -Mathf.Clamp(translation.x, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.RightThruster, _rightThrusterValue);

        float _leftThrusterValue = Mathf.Clamp(translation.x, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.LeftThruster, _leftThrusterValue);

        float _upThrusterValue = -Mathf.Clamp(translation.y, -Mathf.Infinity, 0);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.UpThruster, _upThrusterValue);

        float _downThrusterValue = Mathf.Clamp(translation.y, 0, Mathf.Infinity);
        _thrustersManager.ChangeThrusterValues(_thrustersManager.DownThruster, _downThrusterValue);
    }


    private void InitialiseTranslation()
    {
        _enterInTranslationControlMode = false;

        _handInitialPosition = _translationController.transform.localPosition;

        ChangeShipMarkerDisplay();
    }

    private void InitialiseRotation()
    {
        _enterInRotationControlMode = false;

        _handInitialRotation = _rotationController.transform.localRotation;

        ChangeShipMarkerDisplay();
    }

    private Vector3 LimitTranslationValue(Vector3 translation) // Limits the translation input values inside a sphere.
    {
        float distance = translation.magnitude;
        if (distance >= _maxTranslationInputValue)
        {
            translation *= _maxTranslationInputValue / distance;
        }
        return translation;
    }


    private void ChangeShipMarkerDisplay()
    {
        if (_rotationController != null || _translationController != null)
        {
            _shipMarker.SetActive(true);

            if (_rotationController != null && _translationController != null)
            {
                for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
                {
                    _shipMarkerChildrenRenderers[i].material = _shipMarkerTranslationAndRotationMaterial;
                }
            }
            else if (_rotationController != null && _translationController == null)
            {
                for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
                {
                    _shipMarkerChildrenRenderers[i].material = _shipMarkerRotationMaterial;
                }
            }
            else
            {
                for (int i = 0; i < _shipMarkerChildrenRenderers.Length; i++)
                {
                    _shipMarkerChildrenRenderers[i].material = _shipMarkerTranslationMaterial;
                }
            }
        }
        else if (_rotationController == null && _translationController == null)
        {
            _shipMarker.SetActive(false);
        }
    }
}