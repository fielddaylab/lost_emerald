using System.Collections;
using UnityEngine;
using BeauRoutine;
using UnityEngine.UI;
using ProtoCP;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CameraControls : MonoBehaviour
{
    public GameObject target;
    public Vector3 topOffset;
    public Vector3 sideOffset;
    public Vector3 topUpVector;
    public Vector3 sideUpVector;
    public float shipLength;
    public Button perspectiveButton;
    public PointerListener leftButton;
    public PointerListener rightButton;
    public TMP_Text modeLabel;
    public PointerListener panTarget;

    enum CameraState
    {
        CAMERA_TOP,
        CAMERA_TOP_TO_SIDE,
        CAMERA_SIDE,
        CAMERA_SIDE_TO_TOP,
    }

    CameraState cameraState = CameraState.CAMERA_TOP;
    float sideAngle = 0.0f;
    // each semicircle circumference is pi * |sideOffset|
    // sideAngle from 0 to shipLength: first side of ship length (L to R)
    // sideAngle from shipLength to (shipLength + semicircle): right semicircle
    // sideAngle from (shipLength + semicircle) to (2 * shipLength + semicircle): second side of ship length
    // sideAngle from (2 * shipLength + semicircle) to (2 * shipLength + 2 * semicircle): left semicircle
    // otherwise, normalize to between 0 and (2 * shipLength + 2 * semicircle)
    bool isPanning = false;
    float panPointerStartX = 0.0f;
    float panPointerStartY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraState = CameraState.CAMERA_TOP;
        sideAngle = shipLength / 2.0f; // so we start in the middle of the side
        MoveCamera(1.0f);

        perspectiveButton.onClick.AddListener(SwitchPerspective);
        panTarget.onPointerDown.AddListener(StartPan);
        panTarget.onPointerUp.AddListener(StopPan);
    }

    private void StartPan(PointerEventData arg0)
    {
        isPanning = true;
        panPointerStartX = arg0.position.x;
        panPointerStartY = arg0.position.y;
    }

    private void StopPan(PointerEventData arg0)
    {
        isPanning = false;
    }

    void MoveCamera(float percentTop)
    {
        Vector3 topPosition = target.transform.position + topOffset;
        Vector3 topLookAt = target.transform.position;

        float semiCircumference = Mathf.PI * sideOffset.magnitude;
        float fullTrackLength = 2 * shipLength + 2 * semiCircumference;
        float sideAngleNorm = sideAngle % fullTrackLength;
        if (sideAngleNorm < 0)
        {
            sideAngleNorm += fullTrackLength;
        }

        Vector3 sidePosition = target.transform.position + sideOffset; // overwritten
        Vector3 sideLookAt = target.transform.position; // overwritten
        // TODO get the angle for these by rotating sideOffset around sideUpVector
        Vector3 shipEndL = target.transform.position + new Vector3(shipLength * 0.5f, 0.0f, 0.0f);
        Vector3 shipEndR = target.transform.position + new Vector3(shipLength * -0.5f, 0.0f, 0.0f);
        if (0 <= sideAngleNorm && sideAngleNorm < shipLength)
        {
            // first side of ship length (L to R)
            sideLookAt = shipEndL + (shipEndR - shipEndL) * (sideAngleNorm / shipLength);
            sidePosition = sideLookAt + sideOffset;
        }
        else if (shipLength <= sideAngleNorm && sideAngleNorm < shipLength + semiCircumference)
        {
            // right semicircle
            sideLookAt = shipEndR;
            float degrees = ((sideAngleNorm - shipLength) / semiCircumference) * 180f;
            sidePosition = sideLookAt + Quaternion.Euler(0f, -degrees, 0f) * sideOffset;
        }
        else if (shipLength + semiCircumference <= sideAngleNorm && sideAngleNorm < 2 * shipLength + semiCircumference)
        {
            // second side of ship length
            sideLookAt = shipEndR + (shipEndL - shipEndR) * ((sideAngleNorm - (shipLength + semiCircumference)) / shipLength);
            sidePosition = sideLookAt + Quaternion.Euler(0f, 180f, 0f) * sideOffset;
        }
        else
        {
            // left semicircle
            sideLookAt = shipEndL;
            float degrees = ((sideAngleNorm - (2 * shipLength + semiCircumference)) / semiCircumference) * 180f;
            sidePosition = sideLookAt + Quaternion.Euler(0f, 180f - degrees, 0f) * sideOffset;
        }

        Vector3 animPosition = sidePosition + (topPosition - sidePosition) * percentTop;
        Vector3 animUp = sideUpVector + (topUpVector - sideUpVector) * percentTop;
        Vector3 animLookAt = sideLookAt + (topLookAt - sideLookAt) * percentTop;

        transform.position = animPosition;
        transform.LookAt(animLookAt, animUp);

        if (isPanning)
        {
            float diffX = Input.mousePosition.x - panPointerStartX;
            float diffY = Input.mousePosition.y - panPointerStartY;
            transform.Rotate(transform.up, (diffX / Screen.width) * -70f, Space.World);
            transform.Rotate(transform.right, (diffY / Screen.height) * 70f, Space.World);
        }
    }

    IEnumerator AnimateCamera(CameraState animState, float startValue, float endValue, CameraState endState)
    {
        cameraState = animState;
        yield return Tween.Float(startValue, endValue, (f) => { MoveCamera(f); }, 1.0f).Ease(Curve.CubeInOut);
        cameraState = endState;
    }

    public void SwitchPerspective()
    {
        if (cameraState == CameraState.CAMERA_TOP || cameraState == CameraState.CAMERA_SIDE)
        {
            if (cameraState == CameraState.CAMERA_TOP)
            {
                Routine.Start(this, AnimateCamera(CameraState.CAMERA_TOP_TO_SIDE, 1.0f, 0.0f, CameraState.CAMERA_SIDE));
                modeLabel.SetText("Side View");
            }
            else
            {
                Routine.Start(this, AnimateCamera(CameraState.CAMERA_SIDE_TO_TOP, 0.0f, 1.0f, CameraState.CAMERA_TOP));
                modeLabel.SetText("Overhead View");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraState == CameraState.CAMERA_TOP || cameraState == CameraState.CAMERA_SIDE)
        {
            if (cameraState == CameraState.CAMERA_SIDE && leftButton.IsPressed())
            {
                sideAngle -= 15f * Time.deltaTime;
                MoveCamera(0.0f);
            }
            else if (cameraState == CameraState.CAMERA_SIDE && rightButton.IsPressed())
            {
                sideAngle += 15f * Time.deltaTime;
                MoveCamera(0.0f);
            }
            else
            {
                MoveCamera(cameraState == CameraState.CAMERA_TOP ? 1.0f : 0.0f);
            }
        }
    }
}
