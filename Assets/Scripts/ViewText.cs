using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewText : MonoBehaviour
{
    public Sprite phoneScreen1;
    public Sprite phoneScreen2;
    public Sprite phoneScreen3;
    public Sprite phoneScreen4;

    private int imgNumberCount;

    private void Start()
    {
        GetComponent<Image>().sprite = phoneScreen1;
        imgNumberCount++;
    }

    public void ChangeImages()
    {
        switch (imgNumberCount)
        {
            case 1:
                GetComponent<Image>().sprite = phoneScreen2;
                imgNumberCount++;
                break;
            case 2:
                GetComponent<Image>().sprite = phoneScreen3;
                imgNumberCount++;
                break;
            case 3:
                GetComponent<Image>().sprite = phoneScreen4;
                imgNumberCount++;
                break;
            default:
                GetComponent<SceneSwitch>().GotoDocuments();
                break;
        }
    }
}
