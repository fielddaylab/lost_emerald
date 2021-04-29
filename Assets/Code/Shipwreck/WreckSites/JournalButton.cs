using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class JournalButton : MonoBehaviour
{
    public Button button;
    public float hidePosition = 50f;

    private float animateMultiplier = 1f;
    private Vector3 initialPosition;
    private bool coroutineAllowed;

    private void Start() {
        coroutineAllowed = true;
        initialPosition = button.transform.position;
    }

    private void OnEnable() {
        if(coroutineAllowed){
            StartCoroutine(HideShowButton());
        }
    }

    private IEnumerator HideShowButton() {
        coroutineAllowed = false;
        animateMultiplier *= -1f;
        for (float i = 0f; i <= hidePosition/0.5f; i+= 1f) {
            button.transform.position = new Vector3(button.transform.position.x + 0.5f * animateMultiplier, button.transform.position.y, button.transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        coroutineAllowed = true;
    }

    private void OnDisable() {
        if(coroutineAllowed){
            StartCoroutine(HideShowButton());
        }
    }
}