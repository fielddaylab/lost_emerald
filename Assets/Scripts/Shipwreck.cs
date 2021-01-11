using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipwreck : MonoBehaviour
{
    public float distanceMouseToShip;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var v3Pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        v3Pos = Camera.main.ScreenToWorldPoint(v3Pos);
        distanceMouseToShip = Vector3.Distance(v3Pos, transform.position);
    }
}
