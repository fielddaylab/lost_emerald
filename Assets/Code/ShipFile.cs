using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shipwreck;

public class ShipFile : MonoBehaviour
{
    [SerializeField] GameObject shipFileButton;

    // Start is called before the first frame update
    void Start()
    {
        if ((bool)PlayerProgress.instance?.IsUnlocked("intro-transcript"))
        {
            shipFileButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
