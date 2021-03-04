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
    public PointerListener panTarget;
    public Slider zoomSlider;
    public Button cameraButton;
    public Image cameraFrame;
    public Button photoButton;
    public GameObject photoResult;
    public Button savePhotoButton;
    public GameObject hiddenObject;
    public ThoughtBubble thoughtBubble;
    public string successMessage;
    public float requiredDistance;
    public string unlockKey;

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
    float panPointerSaveX = 0.0f;
    float panPointerSaveY = 0.0f;
    bool photoMode = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraState = CameraState.CAMERA_TOP;
        sideAngle = shipLength / 2.0f; // so we start in the middle of the side
        MoveCamera(1.0f);

        perspectiveButton.onClick.AddListener(SwitchPerspective);
        panTarget.onPointerDown.AddListener(StartPan);
        panTarget.onPointerUp.AddListener(StopPan);

        SetPhotoMode(false);
        cameraButton.onClick.AddListener(SwitchPhotoMode);
        photoButton.onClick.AddListener(TakePhoto);
        savePhotoButton.onClick.AddListener(SavePhoto);
    }

    private void StartPan(PointerEventData arg0)
    {
        if (photoMode)
        {
            isPanning = true;
            panPointerStartX = arg0.position.x;
            panPointerStartY = arg0.position.y;
        }
    }

    private void StopPan(PointerEventData arg0)
    {
        if (photoMode)
        {
            isPanning = false;
            panPointerSaveX += arg0.position.x - panPointerStartX;
            panPointerSaveY += arg0.position.y - panPointerStartY;
        }
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

        if (photoMode)
        {
            float diffX = panPointerSaveX;
            float diffY = panPointerSaveY;
            if (isPanning)
            {
                diffX += Input.mousePosition.x - panPointerStartX;
                diffY += Input.mousePosition.y - panPointerStartY;
            }
            transform.Rotate(transform.up, (diffX / Screen.width) * -70f, Space.World);
            transform.Rotate(transform.right, (diffY / Screen.height) * 70f, Space.World);

            transform.Translate(new Vector3(0f, 0f, zoomSlider.value * 4f), Space.Self);
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
            }
            else
            {
                Routine.Start(this, AnimateCamera(CameraState.CAMERA_SIDE_TO_TOP, 0.0f, 1.0f, CameraState.CAMERA_TOP));
            }
        }
    }

    void SetPhotoMode(bool newPhotoMode)
    {
        photoMode = newPhotoMode;
        if (photoMode)
        {
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            perspectiveButton.gameObject.SetActive(false);
            thoughtBubble.gameObject.SetActive(false);
            zoomSlider.gameObject.SetActive(true);
            cameraFrame.gameObject.SetActive(true);
            photoButton.gameObject.SetActive(true);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
            perspectiveButton.gameObject.SetActive(true);
            thoughtBubble.gameObject.SetActive(true);
            zoomSlider.gameObject.SetActive(false);
            cameraFrame.gameObject.SetActive(false);
            photoButton.gameObject.SetActive(false);
            isPanning = false;
            panPointerStartX = 0f;
            panPointerStartY = 0f;
            panPointerSaveX = 0f;
            panPointerSaveY = 0f;
            zoomSlider.value = 0f;
        }
        photoResult.SetActive(false);
    }

    public void SwitchPhotoMode()
    {
        SetPhotoMode(!photoMode);
    }

    void TakePhoto()
    {
        photoResult.SetActive(true);
        savePhotoButton.gameObject.SetActive(true);
        cameraButton.gameObject.SetActive(false);
        zoomSlider.gameObject.SetActive(false);
        photoButton.gameObject.SetActive(false);
    }

    void CheckHiddenObject(out string message, out string unlockedKey)
    {
        // if we're above the ship, unlock the bird's-eye view
        if (cameraState == CameraState.CAMERA_TOP)
        {
            message = "Great, we got a bird's eye view of the ship!";
            unlockedKey = "photo-birds-eye";
            return;
        }

        // first, check if the center of the hidden object is in the camera frame
        Vector3[] cameraBounds = new Vector3[4];
        cameraFrame.rectTransform.GetWorldCorners(cameraBounds);
        Vector3 cameraBottomLeft = cameraBounds[0];
        Vector3 cameraTopRight = cameraBounds[2];

        Vector3 eggPosition3D = hiddenObject.transform.TransformPoint(hiddenObject.GetComponent<BoxCollider>().center);
        Vector3 eggPosition = GetComponentInChildren<Camera>().WorldToScreenPoint(eggPosition3D);
        bool eggInView = cameraBottomLeft.x < eggPosition.x && eggPosition.x < cameraTopRight.x
            && cameraBottomLeft.y < eggPosition.y && eggPosition.y < cameraTopRight.y;

        // second, check that it is not obstructed
        bool eggVisible = false;
        // could also use Linecast
        if (Physics.Raycast(new Ray(transform.position, eggPosition3D - transform.position), out RaycastHit hit))
        {
            if (hit.collider.gameObject == hiddenObject)
            {
                eggVisible = true;
            }
        }

        // finally, check that we are within some distance of the hidden object
        float distanceToEgg = (eggPosition3D - transform.position).magnitude;

        unlockedKey = null;
        message = "Nothing to see here.";
        if (eggInView && eggVisible)
        {
            if (distanceToEgg < requiredDistance)
            {
                message = successMessage;
                unlockedKey = unlockKey;
            }
            else
            {
                message = "Try getting closer!";
            }
        }

        return;
    }

    void SavePhoto()
    {
        CheckHiddenObject(out _, out string unlockedKey);
        if (unlockedKey != null)
        {
            PlayerProgress.instance?.Unlock(unlockedKey);
        }

        photoResult.SetActive(false);
        savePhotoButton.gameObject.SetActive(false);
        cameraButton.gameObject.SetActive(true);
        zoomSlider.gameObject.SetActive(true);
        photoButton.gameObject.SetActive(true);

        if (unlockedKey != null)
        {
            // close camera after we took a correct photo
            SetPhotoMode(false);
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

        CheckHiddenObject(out string photoMessage, out _);
        photoResult.GetComponentInChildren<TextMeshProUGUI>().text = photoMessage;

        if(photoMessage == "Try getting closer!" && savePhotoButton.isActiveAndEnabled)
        {
            savePhotoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Retry";
        }
        else
        {
            savePhotoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Save";
        }
    }
}
