using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSymbol : MonoBehaviour
{
    public string notificationKey;

    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance.RegisterNotification(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
