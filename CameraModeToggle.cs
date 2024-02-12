using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;
using StarterAssets;

public class CameraModeToggle : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;

    [SerializeField]
    private GameObject[] textures;

    [SerializeField]
    private bool StartWith3rdPerson;

    private bool _isThirdPerson;
    private bool IsThirdPerson
    {
        get => _isThirdPerson;
        set
        {
            _isThirdPerson = value;
            SetTextureState(_isThirdPerson);

            if (_isThirdPerson)
            {
                cinemachine3RdPersonFollow.CameraDistance = 2.5f;
                cinemachine3RdPersonFollow.VerticalArmLength = 0.3f;
            }
            else
            {
                cinemachine3RdPersonFollow.CameraDistance = 0f;
                cinemachine3RdPersonFollow.VerticalArmLength = 0.1f;
            }

        }
    }

    private bool isForceFirstPerson;

    private void Awake()
    {
        cinemachine3RdPersonFollow = cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        IsThirdPerson = StartWith3rdPerson;
    }
    private void OnEnable()
    {
        StarterAssetsInputs.ToggleCam += ToggleCameraPerspectiveChange;
        SkiLift.BoardLift += SetFirstPersonState;
        CinematicChair.SitOnChair += SetFirstPersonState;
        Helicopter.HeliState += SetFirstPersonState;
    }

    private void OnDisable()
    {
        StarterAssetsInputs.ToggleCam -= ToggleCameraPerspectiveChange;
        SkiLift.BoardLift -= SetFirstPersonState;
        CinematicChair.SitOnChair -= SetFirstPersonState;
        Helicopter.HeliState -= SetFirstPersonState;
    }

    private void SetFirstPersonState(bool obj)
    {
   
        isForceFirstPerson = obj;

        if (isForceFirstPerson)
        {
            ForceFirstPerson();
        }
        else
        {

            IsThirdPerson = IsThirdPerson;


        }


    }
    //pprivate methjods
    private void ToggleCameraPerspectiveChange()
    {
        if (isForceFirstPerson)
            return;
        IsThirdPerson = !IsThirdPerson;
    }

    private void SetTextureState(bool isThirdPerson)
    {
        foreach (GameObject text in textures)
        {
            text.SetActive(isThirdPerson);
        }
    }


    private void ForceFirstPerson()
    {
        cinemachine3RdPersonFollow.CameraDistance = 0f;
        cinemachine3RdPersonFollow.VerticalArmLength = 0f;

        SetTextureState(false);
    }
}
