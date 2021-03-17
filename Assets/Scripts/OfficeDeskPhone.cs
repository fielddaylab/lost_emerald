using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficeDeskPhone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerProgress.instance.IsUnlocked("intro-transcript"))
        {
            Button phone = GetComponent<Button>();
            phone.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
