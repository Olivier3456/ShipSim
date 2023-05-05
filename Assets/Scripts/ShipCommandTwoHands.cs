using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ShipCommandTwoHands : MonoBehaviour
{
    [SerializeField] private XRBaseController _translationController;
    [SerializeField] private XRBaseController _rotationController;
    [SerializeField] private GameObject _translationHand;
    [SerializeField] private GameObject _rotationHand;
    [SerializeField] private GameObject _translationZeroPointMarker;
    [SerializeField] private GameObject _rotationZeroPointMarker;
    [SerializeField] private GameObject _translationShipMarker;
    [SerializeField] private GameObject _rotationShipMarker;

   // [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Rigidbody _shipRigidbody;

    [SerializeField] private float _translationsSensibility = 1.0f;
    [SerializeField] private float _rotationsSensibility = 1.0f;

    [SerializeField] private TextMeshProUGUI _debugText1;
    [SerializeField] private TextMeshProUGUI _debugText2;


    private bool _enterInTranslationControlMode = true;
    private bool _enterInRotationControlMode = true;


    private Vector3 _translationForcesToApplyToTheShip;
    private Vector3 _rotationForceToApplyToTheShip;

    private Quaternion rotationOffset;


    //private Vector3[] _zeroPointMarkerAndShipMarkerPositions = new Vector3[2];

  

    private void Update()
    {
        if (_translationController.selectInteractionState.value > 0.5f)
        {
            if (_enterInTranslationControlMode)
            {
                InitialiseTranslationZeroPoint();
                _enterInTranslationControlMode = false;
            }

            _translationShipMarker.transform.localPosition = _translationController.transform.localPosition;

            _translationForcesToApplyToTheShip = _translationShipMarker.transform.localPosition - _translationZeroPointMarker.transform.localPosition;

            


            ShipTranslation();
            


            //_zeroPointMarkerAndShipMarkerPositions[0] = _zeroPointMarker.transform.position;
            //_zeroPointMarkerAndShipMarkerPositions[1] = _shipMarker.transform.position;
            //_lineRenderer.SetPositions(_zeroPointMarkerAndShipMarkerPositions);

        }
        else
        {
            _enterInTranslationControlMode = true;
            _translationZeroPointMarker.SetActive(false);
            _translationShipMarker.SetActive(false);
            _translationHand.SetActive(true);
        }


        if (_rotationController.selectInteractionState.value > 0.5f)
        {
            if (_enterInRotationControlMode)
            {
                InitialiseRotationZeroPoint();
                _enterInRotationControlMode = false;
            }

            _rotationShipMarker.transform.localRotation = _rotationController.transform.localRotation;  // Ce serait bien qu'elle se base sur la rotation de _zeroPointMarker.

            _rotationForceToApplyToTheShip = _rotationShipMarker.transform.localRotation.eulerAngles - _rotationZeroPointMarker.transform.localRotation.eulerAngles;
            CorrectAngle(ref _rotationForceToApplyToTheShip.x);
            CorrectAngle(ref _rotationForceToApplyToTheShip.y);
            CorrectAngle(ref _rotationForceToApplyToTheShip.z);

            ShipRotation();
        }
        else
        {
            _enterInRotationControlMode = true;
            _rotationZeroPointMarker.SetActive(false);
            _rotationShipMarker.SetActive(false);
            _rotationHand.SetActive(true);
        }


        /* Pour debugger: */
        if (_translationController.activateInteractionState.value > 0.5f || _rotationController.activateInteractionState.value > 0.5f) SceneManager.LoadSceneAsync(0);
    }

    private float CorrectAngle(ref float angle)
    {        
        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;
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


    private void InitialiseTranslationZeroPoint()
    {
        _translationHand.SetActive(false);
        _translationZeroPointMarker.SetActive(true);
        _translationShipMarker.SetActive(true);
        _translationZeroPointMarker.transform.localPosition = _translationController.transform.localPosition;
        _translationZeroPointMarker.transform.rotation = _shipRigidbody.transform.rotation;        
    }

    private void InitialiseRotationZeroPoint()
    {
        _rotationHand.SetActive(false);
        _rotationZeroPointMarker.SetActive(true);
        _rotationShipMarker.SetActive(true);

        _rotationZeroPointMarker.transform.localPosition = _rotationController.transform.localPosition;
        _rotationZeroPointMarker.transform.rotation = _rotationController.transform.rotation;

        _rotationShipMarker.transform.localPosition = _rotationController.transform.localPosition;       
    }
}
