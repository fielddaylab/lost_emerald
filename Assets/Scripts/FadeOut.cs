using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] GameObject toShow;
    [SerializeField] GameObject toHide;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (toShow != null){
                toShow.SetActive(true);
            }
            toHide.SetActive(false);

        }
    }
}
