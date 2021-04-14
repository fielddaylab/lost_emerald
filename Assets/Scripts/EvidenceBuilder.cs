using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceBuilder : MonoBehaviour
{
    public Image slotTop;
    public Image slotBottom;
    public TabPanel tabPanel;
    public GameObject evidencePopupContainer;
    public Image evidencePopupImage;

    private string keyTop;
    private string keyBottom;

    public enum Slot
    {
        SLOT_TOP,
        SLOT_BOTTOM
    }

    public void SetSlot(Slot slot, Sprite sprite, string key)
    {
        if (slot == Slot.SLOT_TOP)
        {
            slotTop.sprite = sprite;
            keyTop = key;
        }
        else if (slot == Slot.SLOT_BOTTOM)
        {
            slotBottom.sprite = sprite;
            keyBottom = key;
        }
        if(keyBottom == "photo-birds-eye" || keyTop == "photo-birds-eye")
        {
            PlayerProgress.instance.Unlock("photo-birds-eye-dragged");
        }
        else if (keyBottom == "photo-iron-knees" || keyTop == "photo-iron-knees")
        {
            PlayerProgress.instance.Unlock("photo-iron-knees-dragged");
        }
        ComputeMatch();
    }

    private void ComputeMatch()
    {
        string unlockKey = null;
        if (keyTop == "photo-birds-eye" && keyBottom == "type-canaller" || keyBottom == "photo-birds-eye" && keyTop == "type-canaller")
        {
            unlockKey = "verified-canaller";
            PlayerProgress.instance.TemporaryBubble("Aha! It's a canaller!");
            evidencePopupImage.sprite = Resources.Load<Sprite>("evidence-canaller");
            evidencePopupContainer.SetActive(true);
        }
        else if (keyTop == "photo-iron-knees" && keyBottom == "diagram-iron-knees" || keyBottom == "photo-iron-knees" && keyTop == "diagram-iron-knees")
        {
            unlockKey = "verified-loretta";
            PlayerProgress.instance.TemporaryBubble("Aha! It's the Loretta!");
            evidencePopupImage.sprite = Resources.Load<Sprite>("evidence-loretta");
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
