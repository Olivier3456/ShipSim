using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Adapté de ButtonFollowVisual de Valem.

public class ButtonPerso : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localAxis;
    public float resetSpeed = 5;
    public float followAngleTreshold = 45;

    private bool _freeze = false;

    private Vector3 _initialLocalPosition;
    
    public bool buttonIsOn = false;
    private AudioSource _audioSource;
    public AudioClip audioClipOn;
    public AudioClip audioClipOff;
    public Renderer renderer;
    public Material materialOn;
    public Material materialOff;
    


    private Vector3 _offset;
    private Transform _pokeAttachTransform;

    private XRBaseInteractable _interactable;
    private bool _isFollowing = false;

    void Start()
    {
        _initialLocalPosition = visualTarget.localPosition;

        _interactable = GetComponent<XRBaseInteractable>();
        _interactable.hoverEntered.AddListener(Follow);
        _interactable.hoverExited.AddListener(Reset);
        _interactable.selectEntered.AddListener(Freeze);

        _audioSource = GetComponent<AudioSource>();        
        if (buttonIsOn) renderer.material = materialOn;
        else renderer.material = materialOff;
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            _pokeAttachTransform = interactor.attachTransform;
            _offset = visualTarget.position - _pokeAttachTransform.position;

            float pokeAngle = Vector3.Angle(_offset, visualTarget.TransformDirection(localAxis));

            if (pokeAngle < followAngleTreshold)
            {
                _isFollowing = true;
                _freeze = false;
            }
        }
    }

    public void Reset(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            _isFollowing = false;
            _freeze = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            _freeze = true;

            buttonIsOn = !buttonIsOn;

            if (buttonIsOn)
            {
                _audioSource.PlayOneShot(audioClipOn);
                renderer.material = materialOn;
            }
            else

            {
                _audioSource.PlayOneShot(audioClipOff);
                renderer.material = materialOff;
            }            
        }
    }


    void Update()
    {
        if (_freeze) return;


        if (_isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(_pokeAttachTransform.position + _offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);
            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
        else
        {
                visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, _initialLocalPosition, Time.deltaTime * resetSpeed);
        }
    }
}
