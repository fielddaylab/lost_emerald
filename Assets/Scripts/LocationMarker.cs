using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMarkerFound()
    {
        PlayerProgress.instance.Unlock("location-marker-found");
    }
}
