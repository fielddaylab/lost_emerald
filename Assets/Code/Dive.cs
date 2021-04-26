using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shipwreck;

public class Dive : MonoBehaviour
{
    public static bool mouseOnDive;
    public GameObject buoy;
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

    public void SonarComplete()
    {
        if (!PlayerProgress.instance.IsUnlocked("sonar-complete"))
        {
            PlayerProgress.instance.Unlock("sonar-complete");
            Logging.instance?.LogScanComplete();
        }
    }

    private void BeenToDive()
    {
        OffDive();
        if (!PlayerProgress.instance.IsUnlocked("been-to-dive"))
        {
            PlayerProgress.instance.Unlock("been-to-dive");
            Logging.instance?.LogDiveStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Logging.instance?.LogScanPercentageChange(Ship.count);

        if (Ship.count > 80)
        {
            SonarComplete();
            GetComponent<Button>().interactable = true;
            GetComponent<Button>().onClick.AddListener(BeenToDive);
            buoy.SetActive(true);
        }
    }
}
