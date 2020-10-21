using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

public class CameraControls : MonoBehaviour
{
    public GameObject target;
    public Vector3 topOffset;
    public Vector3 sideOffset;

    enum CameraState
    {
        CAMERA_TOP,
        CAMERA_TOP_TO_SIDE,
        CAMERA_SIDE,
        CAMERA_SIDE_TO_TOP,
    }

    CameraState cameraState = CameraState.CAMERA_TOP;
    float sideAngle = 0.0f; // degrees

    // Start is called before the first frame update
    void Start()
    {
        cameraState = CameraState.CAMERA_TOP;
        MoveCamera(1.0f);
    }

    void MoveCamera(float percentTop)
    {
        Vector3 topPosition = target.transform.position + topOffset;
        Vector3 rotatedOffset = Quaternion.Euler(0.0f, sideAngle, 0.0f) * sideOffset;
        Vector3 sidePosition = target.transform.position + rotatedOffset;
        Vector3 animPosition = sidePosition + (topPosition - sidePosition) * percentTop;

        Vector3 topUp = new Vector3(1.0f, 0.0f, 1.0f).normalized;
        Vector3 sideUp = new Vector3(0.0f, 1.0f, 0.0f).normalized;
        Vector3 animUp = sideUp + (topUp - sideUp) * percentTop;

        transform.position = animPosition;
        transform.LookAt(target.transform, animUp);
    }

    IEnumerator AnimateCamera(CameraState animState, float startValue, float endValue, CameraState endState)
    {
        cameraState = animState;
        yield return Tween.Float(startValue, endValue, (f) => { MoveCamera(f); }, 1.0f).Ease(Curve.CubeInOut);
        cameraState = endState;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraState == CameraState.CAMERA_TOP || cameraState == CameraState.CAMERA_SIDE)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (cameraState == CameraState.CAMERA_TOP)
                {
                    Routine.Start(this, AnimateCamera(CameraState.CAMERA_TOP_TO_SIDE, 1.0f, 0.0f, CameraState.CAMERA_SIDE));
                }
                else
                {
                    Routine.Start(this, AnimateCamera(CameraState.CAMERA_SIDE_TO_TOP, 0.0f, 1.0f, CameraState.CAMERA_TOP));
                }
            }
            else if (cameraState == CameraState.CAMERA_SIDE && Input.GetKey(KeyCode.LeftArrow))
            {
                sideAngle += 1.0f;
                MoveCamera(0.0f);
            }
            else if (cameraState == CameraState.CAMERA_SIDE && Input.GetKey(KeyCode.RightArrow))
            {
                sideAngle -= 1.0f;
                MoveCamera(0.0f);
            }
        }
    }
}
