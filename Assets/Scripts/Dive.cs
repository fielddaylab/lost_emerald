﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dive : MonoBehaviour
{
    public static bool mouseOnDive;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnDive()
    {
        mouseOnDive = true;
    }

    public void OffDive()
    {
        mouseOnDive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ship.count > 80)
        {
            GetComponent<Button>().interactable = true;
            PlayerProgress.instance.Unlock("sonar-complete");
        }
    }
}
