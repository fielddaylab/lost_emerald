using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogWhenViewed : MonoBehaviour
{
    public string logKey;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LogView()
    {
        PlayerProgress.instance.Unlock(logKey);
    }
}
