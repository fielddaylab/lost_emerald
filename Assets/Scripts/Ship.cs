using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Vector3 dragOrigin;
    private float step;
    public static int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        step = Time.deltaTime / Mathf.PI;
        PlayerProgress.instance.Unlock("ship-on-lake");
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

        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width
            || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            // prevent the ship from going off screen
            return;
        }

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - transform.position);

        if (!Dive.mouseOnDive)
        {
            Vector3 move = new Vector3(pos.x * 30, pos.y * 30, 0);
            transform.Translate(move, Space.World);
            transform.rotation = Quaternion.LookRotation(transform.forward, pos);
        }
        

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((Input.mousePosition - transform.position).magnitude > 175 && other.tag == "MaskingBlock")
        {
            other.gameObject.SetActive(false);
            count++;
        }
        else
        {
            int genRand = Random.Range(0, 4);
            if (genRand == 0)
            {
                other.gameObject.SetActive(false);
                count++;
            }
        }
    }
}
