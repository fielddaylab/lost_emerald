using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            evidencePopupImage1.sprite = Resources.Load<Sprite>("birds-eye-photo-new");
            evidencePopupImage2.sprite = Resources.Load<Sprite>("ship-type-canaller");
            evidencePopupName.text = "Canaller";
            evidencePopupCaption.text = "The photo of the wreck we took from above matches the shape of a Canaller.";
            evidencePopupContainer.SetActive(true);
        }
        else if (keyTop == "photo-iron-knees" && keyBottom == "diagram-iron-knees" || keyBottom == "photo-iron-knees" && keyTop == "diagram-iron-knees")
        {
            unlockKey = "verified-loretta";
            PlayerProgress.instance.TemporaryBubble("Aha! It's the Loretta!");
            evidencePopupImage1.sprite = Resources.Load<Sprite>("iron-knees-photo-new");
            evidencePopupImage2.sprite = Resources.Load<Sprite>("DialogImages/knees-loretta");
            evidencePopupName.text = "The Loretta";
            evidencePopupCaption.text = "The iron knees on the wreck prove that the ship is the Loretta.";
            evidencePopupContainer.SetActive(true);
        }

        if (unlockKey == null)
        {
            if (keyTop != null && keyBottom != null)
            {
                GetComponent<Image>().color = new Color(255f / 255f, 133f / 255f, 132f / 255f);
            }
            else
            {
                GetComponent<Image>().color = new Color(0f / 255f, 244f / 255f, 255f / 255f);
            }
        }
        else
        {
            GetComponent<Image>().color = new Color(134f / 255f, 235f / 255f, 155f / 255f);
            PlayerProgress.instance.Unlock(unlockKey);
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
