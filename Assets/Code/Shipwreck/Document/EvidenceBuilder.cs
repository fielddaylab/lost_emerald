using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shipwreck;

public class EvidenceBuilder : MonoBehaviour
{
    public Image slotTop;
    public Image slotBottom;
    public TabPanel tabPanel;
    public GameObject evidencePopupContainer;
    public Image evidencePopupImage1;
    public Image evidencePopupImage2;
    public TextMeshProUGUI evidencePopupName;
    public TextMeshProUGUI evidencePopupCaption;

    private string keyTop;
    private string keyBottom;

    public enum Slot
    {
        SLOT_TOP,
        SLOT_BOTTOM
    }

    public void SetSlot(Slot slot, Sprite sprite, string key)
    {
        TextMeshProUGUI slotText = null;
        if (slot == Slot.SLOT_TOP)
        {
            slotTop.sprite = sprite;
            slotText = slotTop.GetComponentInChildren<TextMeshProUGUI>(true);
            keyTop = key;
        }
        else if (slot == Slot.SLOT_BOTTOM)
        {
            slotBottom.sprite = sprite;
            slotText = slotBottom.GetComponentInChildren<TextMeshProUGUI>(true);
            keyBottom = key;
        }

        if (sprite)
        {
            slotText.gameObject.SetActive(false);
        }
        else
        {
            slotText.text = key;
            slotText.gameObject.SetActive(true);
        }

        if (keyBottom == "photo-birds-eye" || keyTop == "photo-birds-eye")
        {
            PlayerProgress.instance.Unlock("photo-birds-eye-dragged");
        }
        else if (keyBottom == "photo-ship-name" || keyTop == "photo-ship-name")
        {
            PlayerProgress.instance.Unlock("photo-ship-name-dragged");
        }
        ComputeMatch();
    }

    private void ComputeMatch()
    {
        // TODO move this logic into LevelBase and its subclasses
        string unlockKey = null;
        if (keyTop == "photo-birds-eye" && keyBottom == "type-canaller" || keyBottom == "photo-birds-eye" && keyTop == "type-canaller")
        {
            unlockKey = "verified-canaller";
            PlayerProgress.instance.TemporaryBubble("It's a canaller!");
            evidencePopupImage1.sprite = Resources.Load<Sprite>("birds-eye-photo-new");
            evidencePopupImage2.sprite = Resources.Load<Sprite>("ship-type-canaller");
            evidencePopupName.text = "Canaller";
            evidencePopupCaption.text = "The ship photo and Ship Type card match!";
            evidencePopupContainer.SetActive(true);
        }
        else if (keyTop == "photo-ship-name" && keyBottom == "wreck-loretta" || keyBottom == "photo-ship-name" && keyTop == "wreck-loretta")
        {
            unlockKey = "verified-loretta";
            PlayerProgress.instance.TemporaryBubble("Aha! It's the Loretta!");
            evidencePopupImage1.sprite = Resources.Load<Sprite>("photo-ship-name");
            evidencePopupImage2.sprite = Resources.Load<Sprite>("photo-ship-name");
            evidencePopupName.text = "The Loretta";
            evidencePopupCaption.text = "Our photo of the ship's name is missing a few letters, but it says Loretta!";
            evidencePopupContainer.SetActive(true);
        }
        else if (keyTop == "raffle" && keyBottom == "photo-ice-cream" || keyBottom == "raffle" && keyTop == "photo-ice-cream")
        {
            unlockKey = "verified-raffle";
            // TODO: Add the updated ice cream stuff
            PlayerProgress.instance.TemporaryBubble("Aha! It's the Loretta!");
            evidencePopupImage1.sprite = Resources.Load<Sprite>("photo-ship-name");
            evidencePopupImage2.sprite = Resources.Load<Sprite>("photo-ship-name");
            evidencePopupName.text = "The Loretta";
            evidencePopupCaption.text = "Our photo of the ship's name is missing a few letters, but it says Loretta!";
            evidencePopupContainer.SetActive(true);

        }
        

        if (unlockKey == null)
        {
            if (keyTop != null && keyBottom != null)
            {
                GetComponent<Image>().color = new Color(255f / 255f, 133f / 255f, 132f / 255f);
                ShipAudio.AudioMgr.Instance.PostEvent("l1_ui_EvidenceWrong");
            }
            else
            {
                GetComponent<Image>().color = new Color(0f / 255f, 244f / 255f, 255f / 255f);
                ShipAudio.AudioMgr.Instance.PostEvent("l1_ui_EvidenceRight");
            }
        }
        else
        {
            GetComponent<Image>().color = new Color(134f / 255f, 235f / 255f, 155f / 255f);
            PlayerProgress.instance.Unlock(unlockKey);
            ShipAudio.AudioMgr.Instance.PostEvent("l1_ui_EvidenceRight");
        }
    }

    public void Exit()
    {
        tabPanel.SelectTab(0);
        slotTop.sprite = null;
        slotBottom.sprite = null;
        keyTop = null;
        keyBottom = null;
        ComputeMatch();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
