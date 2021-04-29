using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Shipwreck;

public class DocumentButton : MonoBehaviour
{
    public string targetKey;
    public string originalText;

    // Start is called before the first frame update
    void Start()
    {
        originalText = GetComponentInChildren<TextMeshProUGUI>().text;
        PlayerProgress.instance?.SetDocumentPresence(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
