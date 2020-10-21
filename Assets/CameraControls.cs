using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject target;
    public Vector3 topOffset;
    public Vector3 sideOffset;

    bool lookingTop;
    float sideAngle = 0.0f; // degrees

    // Start is called before the first frame update
    void Start()
    {
        ViewTop();
    }

    void ViewTop()
    {
        lookingTop = true;
        transform.position = target.transform.position + topOffset;
        transform.LookAt(target.transform);
    }

    void ViewSide()
    {
        lookingTop = false;
        Vector3 rotatedOffset = Quaternion.Euler(0.0f, sideAngle, 0.0f) * sideOffset;
        transform.position = target.transform.position + rotatedOffset;
        transform.LookAt(target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lookingTop)
            {
                ViewSide();
            }
            else
            {
                ViewTop();
            }
        }
        else if (!lookingTop && Input.GetKey(KeyCode.LeftArrow))
        {
            sideAngle += 1.0f;
            ViewSide();
        }
        else if (!lookingTop && Input.GetKey(KeyCode.RightArrow))
        {
            sideAngle -= 1.0f;
            ViewSide();
        }
    }
}
