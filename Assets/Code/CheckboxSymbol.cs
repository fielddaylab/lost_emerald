using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shipwreck;

public class CheckboxSymbol : MonoBehaviour
{
    public string checkboxKey;

    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance.RegisterCheckbox(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
