using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipOutButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance?.ShipOutButton(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
