using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dive : MonoBehaviour
{
    [SerializeField]private Image background;
    [SerializeField] private Text dive;
    // Start is called before the first frame update
    void Start()
    {
        background.enabled = false;
        dive.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ship.count > 80)
        {
            background.enabled = true;
            dive.enabled = true;
        }
    }
}
