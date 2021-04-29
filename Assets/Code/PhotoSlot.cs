using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shipwreck;

public class PhotoSlot : MonoBehaviour
{
    public string unlockKey;
    public string infoKey;
    public string infoDisplay;

    private Sprite originalSprite;

    // Start is called before the first frame update
    void Start()
    {
        originalSprite = GetComponent<Image>().sprite;
        PlayerProgress.instance?.SetPhotoPresence(this);
    }

    public void SetLocked()
    {
        // GetComponent<Image>().color = Color.gray;
        // GetComponent<Image>().sprite = null;
        // foreach (var item in GetComponentsInChildren<Transform>())
        // {
        //     if (item.gameObject != this.gameObject)
        //     {
        //         item.gameObject.SetActive(false);
        //     }
        // }
        this.gameObject.SetActive(false);

    }

    public void SetUnlocked()
    {
        // GetComponent<Image>().color = Color.white;
        // GetComponent<Image>().sprite = originalSprite;
        // foreach (var item in GetComponentsInChildren<Transform>())
        // {
        //     if (item.gameObject != this.gameObject)
        //     {
        //         item.gameObject.SetActive(true);
        //     }
        // }
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
