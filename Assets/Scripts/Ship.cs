using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Vector3 dragOrigin;
    private float step;
    public static int count = 0;
    private int speed;

    // Start is called before the first frame update
    void Start()
    {
        step = Time.deltaTime / Mathf.PI;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        if ((Input.mousePosition - transform.position).magnitude > 100)
        {
            speed = 30;
        }

        else
        {
            speed = 2;
        }
        Debug.Log(speed);

        Vector3 move = new Vector3(pos.x * speed, pos.y * speed, 0);

        transform.Translate(move, Space.World);
        transform.rotation = Quaternion.LookRotation(transform.forward, pos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (speed == 2 && other.tag == "MaskingBlock")
        {
            Destroy(other.gameObject);
            count++;
        }
        else
        {
            int genRand = Random.Range(0, 3);
            if (genRand == 0)
            {
                Destroy(other.gameObject);
                count++;
            }
        }
    }
}
