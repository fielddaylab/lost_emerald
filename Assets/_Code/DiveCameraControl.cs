using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class DiveCameraControl : MonoBehaviour
{
    public CinemachineVirtualCamera thisVCam;
    public Collider clickTarget;
    private Camera mainCamera;
    private CinemachineVirtualCamera[] allVCams;

    void Awake()
	{
        mainCamera = Camera.main;
        allVCams = FindObjectsOfType<CinemachineVirtualCamera>();

    }

	void Update()
    {
        var currentMouse = Mouse.current;
        if (currentMouse == null)
            return;

        if (currentMouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
			{
                if (hit.collider == clickTarget)
				{
                    switchToThisCam();
				}
			}

        }

    }

    void switchToThisCam()
	{
        thisVCam.Priority = 10;
        foreach (CinemachineVirtualCamera vCam in allVCams)
        {
            if (vCam != thisVCam)
			{
                vCam.Priority = 0;
            }
        }
    }
}
