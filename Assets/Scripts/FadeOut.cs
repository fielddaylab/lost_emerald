using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] GameObject toShow;
    [SerializeField] GameObject toHide;
    [SerializeField] bool sonar;
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
            if (sonar)
            {
                toShow.SetActive(true);
                toHide.SetActive(false);
            }
            else
            {
                PlayerProgress.instance.ClearRegistrations();
                SceneManager.LoadScene("LaSalleTestScene_RealtimeLighting");
            }
        }
    }
}
