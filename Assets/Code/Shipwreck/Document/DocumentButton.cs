using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shipwreck;

public class DocumentButton : MonoBehaviour
{
    public string targetKey;
    public string originalText;
    public Button button;
    public NotificationSymbol notification;

    // Start is called before the first frame update
    void Start()
    {
        originalText = GetComponentInChildren<TextMeshProUGUI>().text;
        PlayerProgress.instance?.SetDocumentPresence(this);
        button = GetComponent<Button>();
        if(notification != null) {
            button.onClick.AddListener(DocumentClicked);
        }
    }

    void DocumentClicked() {
        PlayerProgress.instance.Unlock(notification.notificationKey + "-clicked");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
