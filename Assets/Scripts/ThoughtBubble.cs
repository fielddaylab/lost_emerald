using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance.SetThoughtBubble(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        PlayerProgress.instance.ClearThoughtBubble(this);
    }
}
