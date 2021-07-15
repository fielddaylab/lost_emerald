using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class DiveCameraControl : MonoBehaviour
{
    public CinemachineVirtualCamera thisVCam;
    public Collider clickTarget;
    public GameObject clickTargetArt;
    public Color clickTargetColor;
    public Color clickTargetSelectedColor;
    public bool startCamera;
    public float startingCameraDelay;
    private Camera mainCamera;
    private CinemachineVirtualCamera[] allVCams;
    private GameObject[] allClickTargets;

    void Awake()
	{
        mainCamera = Camera.main;
        allVCams = FindObjectsOfType<CinemachineVirtualCamera>();
        allClickTargets = GameObject.FindGameObjectsWithTag("ClickTarget");

        if (startCamera == true)
        {
            StartCoroutine(setStartingCam());
        }
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

    IEnumerator setStartingCam()
	{
        yield return new WaitForSeconds(startingCameraDelay);
        switchToThisCam();
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

        // GetCompoment probably should not be called every time the cam switches - not sure how to cash the material instance :)
        clickTargetArt.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", clickTargetSelectedColor);

        foreach (GameObject target in allClickTargets)
        {
            if (target != clickTargetArt)
            {
                target.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", clickTargetColor);
            }
        }
    }
}
