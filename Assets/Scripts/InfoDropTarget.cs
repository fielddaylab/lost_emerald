using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoDropTarget : MonoBehaviour
{
    public string targetKey;
    public TextMeshProUGUI infoLabel;
    public TextMeshProUGUI sourceLabel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance?.FillInfo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
