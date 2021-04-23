using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    public Slider slider;
    private float currentValue = 0f;
    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            currentValue = value;
            slider.value = currentValue;
        }
    }

    // Use this for initialization
    void Start()
    {
        CurrentValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentValue = Ship.count/80f;
    }
}
