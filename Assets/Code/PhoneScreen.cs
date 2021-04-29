using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using Shipwreck;
using Shipwreck.Scene;

public class PhoneScreen : MonoBehaviour
{
    public GameObject[] texts;
    public string defaultDialogKey;
    public TextMeshProUGUI headerLabel;
    public Image characterImage;
    public GameObject replyArea;
    public Button sendButton;
    public ScrollRect scrollArea;
    public GameObject templateLineThem;
    public GameObject templateLineUs;
    public Image templateImageThem;
    public GameObject templateExit;

    private string[] dialogFile;

    private void Start()
    {
        string dialogKey = defaultDialogKey;
        if (PlayerProgress.instance != null)
        {
            string gotKey = PlayerProgress.instance.GetDialogKey();
            if (gotKey != null)
            {
                dialogKey = gotKey;
            }
        }

        TextAsset dialog = Resources.Load("Dialogs/" + dialogKey) as TextAsset;
        dialogFile = Regex.Split(dialog.text, "\r\n|\r|\n");

        templateLineThem.SetActive(false);
        templateImageThem.gameObject.SetActive(false);
        templateLineUs.SetActive(false);
        replyArea.SetActive(false);
        sendButton.gameObject.SetActive(false);
        templateExit.SetActive(false);

        StartCoroutine(ContinueConversation(0));
    }

    private void ReflowScroll()
    {
        const float spacing = 15;

        float fullHeight = 0;
        for (int i = 0; i < scrollArea.content.childCount; i++)
        {
            RectTransform child = scrollArea.content.GetChild(i).GetComponent<RectTransform>();
            fullHeight += child.sizeDelta.y + spacing;
        }
        scrollArea.content.sizeDelta = new Vector2(scrollArea.content.sizeDelta.x, fullHeight);

        float y = 0;
        for (int i = 0; i < scrollArea.content.childCount; i++)
        {
            RectTransform child = scrollArea.content.GetChild(i).GetComponent<RectTransform>();
            child.localPosition = new Vector2(child.localPosition.x, y - child.sizeDelta.y);
            y -= child.sizeDelta.y + 15;
        }
    }
    
    private IEnumerator SendMessage(int i)
    {
        replyArea.SetActive(false);
        sendButton.gameObject.SetActive(false);

        string contents = dialogFile[i + 1];
        GameObject newLine = Instantiate(templateLineUs, scrollArea.content, true);
        TextMeshProUGUI lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform lineRect = newLine.GetComponentInChildren<RectTransform>();
        lineText.text = contents;
        float newLineHeight = lineText.GetPreferredValues(contents, lineRect.sizeDelta.x - 10, 0).y + 10;
        lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, newLineHeight);
        lineRect.localPosition = new Vector2(scrollArea.content.rect.width, 0);
        ReflowScroll();
        scrollArea.verticalNormalizedPosition = 0f;
        newLine.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return ContinueConversation(i + 3);
    }

    private IEnumerator ContinueConversation(int i)
    {
        if (i >= dialogFile.Length)
        {
            yield break; // TODO go back to documents?
        }

        string line = dialogFile[i];
        string contents = dialogFile[i + 1];
        if (line == "char:")
        {
            // set image and name of character
            if (contents == "lou")
            {
                headerLabel.text = "Conversation with Lou the Pilot:";
                ShipAudio.AudioMgr.Instance.SetMusic("l1_amb_airfield", 0.5f);
            }
            else if (contents == "amy")
            {
                headerLabel.text = "Conversation with Amy the Archivist:";
                ShipAudio.AudioMgr.Instance.SetMusic("l1_amb_library", 0.5f);
            }
            else if (contents == "rusty")
            {
                headerLabel.text = "Conversation with Rusty the Shipbuilder:";
                ShipAudio.AudioMgr.Instance.SetMusic("l1_amb_shipyard", 0.5f);
            }

            Sprite sprite = Resources.Load<Sprite>("Characters/" + contents);
            characterImage.sprite = sprite;
        }
        else if (line == "them:")
        {
            // other person says a line
            GameObject newLine = Instantiate(templateLineThem, scrollArea.content, true);
            TextMeshProUGUI lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform lineRect = newLine.GetComponentInChildren<RectTransform>();
            lineText.text = contents;
            float newLineHeight = lineText.GetPreferredValues(contents, lineRect.sizeDelta.x - 10, 0).y + 10;
            lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, newLineHeight);
            lineRect.localPosition = new Vector2(0, 0);
            ReflowScroll();
            scrollArea.verticalNormalizedPosition = 0f;
            newLine.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        else if (line == "us:")
        {
            // put a line in the box for us to hit Send
            replyArea.GetComponentInChildren<TextMeshProUGUI>().text = contents;
            replyArea.SetActive(true);
            sendButton.gameObject.SetActive(true);
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(() =>
            {
                StartCoroutine(SendMessage(i));
            });
            yield break;
        }
        else if (line == "image-them:")
        {
            // other person sends an image
            Sprite sprite = Resources.Load<Sprite>("DialogImages/" + contents);
            GameObject newImage = Instantiate(templateImageThem.gameObject, scrollArea.content, true);
            Image img = newImage.GetComponent<Image>();
            img.sprite = sprite;
            RectTransform imgRect = newImage.GetComponentInChildren<RectTransform>();
            imgRect.localPosition = new Vector2(0, 0);
            ReflowScroll();
            scrollArea.verticalNormalizedPosition = 0f;
            newImage.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        else if (line == "unlock:")
        {
            // unlock something in the documents scene
            PlayerProgress.instance?.Unlock(contents);
        }
        else if (line == "exit:")
        {
            // show a button to end the dialog
            GameObject newLine = Instantiate(templateExit, scrollArea.content, true);
            TextMeshProUGUI lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform lineRect = newLine.GetComponentInChildren<RectTransform>();
            lineText.text = contents;
            lineRect.localPosition = new Vector2(0, 0);
            ReflowScroll();
            scrollArea.verticalNormalizedPosition = 0f;
            newLine.GetComponent<Button>().onClick.AddListener(() =>
            {
                if ((bool)PlayerProgress.instance?.ChapterComplete())
                {
                    PlayerProgress.instance.Unlock("informed-lou");
                    GetComponent<SceneSwitch>().GotoLevelEnding();
                }
                else
                {
                    GetComponent<SceneSwitch>().GotoDocuments(PlayerProgress.instance.GetCurrentLevel());

                }
            });
            newLine.SetActive(true);
            yield break;
        }
        yield return ContinueConversation(i + 3);
    }
}
