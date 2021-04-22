using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSonarScene : MonoBehaviour
{
    public GameObject ship;
    public GameObject lakeMichigan;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerProgress.instance.IsUnlocked("been-to-dive"))
        {
            lakeMichigan.SetActive(false);
            ship.SetActive(true);
        }
        else
        {
            lakeMichigan.SetActive(true);
            ship.SetActive(false);
        }

        Logging.instance.LogOpenMap("loretta");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
