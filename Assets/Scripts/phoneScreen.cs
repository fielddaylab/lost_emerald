using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class phoneScreen : MonoBehaviour
{
    public GameObject[] texts;

    private int textCount = 0;

    private void Start()
    {
        StartCoroutine(TextAppear(1f));
        StartCoroutine(TextAppear(3f));
        StartCoroutine(TextAppear(3f));
        
    }

    IEnumerator TextAppear( float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (textCount < texts.Length)
        {
            texts[textCount].SetActive(true);
            textCount++;
        }
    }
}
