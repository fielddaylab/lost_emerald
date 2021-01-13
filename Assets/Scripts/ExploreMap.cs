using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreMap : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(GameObject toShow)
    {
        toShow.SetActive(true);
    }

    public void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }
}
