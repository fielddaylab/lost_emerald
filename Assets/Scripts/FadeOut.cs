using System.Collections;
using System.Collections.Generic;
using ShipAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    [SerializeField] string audioName;
    [SerializeField] GameObject toShow;
    [SerializeField] GameObject toHide;
    [SerializeField] bool sonar;
    
    AudioHandle audioPlayback;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayback = AudioMgr.Instance.PostEvent(audioName);
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioPlayback.IsPlaying())
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
