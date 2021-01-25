using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress instance;

    public struct InfoEntry
    {
        public string infoKey;
        public string infoDisplay;
        public string sourceDisplay;
    }

    private Dictionary<string, InfoEntry> shipLog = new Dictionary<string, InfoEntry>();
    private string dialogToLoad;
    private HashSet<string> playerUnlocks = new HashSet<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void FillInfo(InfoDropTarget target)
    {
        if (shipLog.TryGetValue(target.targetKey, out InfoEntry entry))
        {
            target.infoLabel.text = entry.infoDisplay;
            target.sourceLabel.text = "From: " + entry.sourceDisplay;
        }
        else
        {
            target.infoLabel.text = "Unknown";
            target.sourceLabel.text = "";
        }
    }

    public void SetPhotoPresence(PhotoSlot target)
    {
        if (playerUnlocks.Contains(target.targetKey))
        {
            // do nothing
        }
        else
        {
            target.GetComponent<Image>().color = Color.gray;
            target.GetComponent<Image>().sprite = null;
        }
    }

    public void SetDocumentPresence(DocumentButton target)
    {
        if (playerUnlocks.Contains(target.targetKey))
        {
            // do nothing
        }
        else
        {
            target.GetComponent<Button>().interactable = false;
            target.GetComponentInChildren<TextMeshProUGUI>().text = "?";
        }
    }

    public void DropInfo(InfoDropTarget target, InfoEntry info)
    {
        shipLog[target.targetKey] = info;
        FillInfo(target);
    }

    public void SetDialogKey(string key)
    {
        dialogToLoad = key;
    }

    public string GetDialogKey()
    {
        string key = dialogToLoad;
        dialogToLoad = null;
        return key;
    }

    public void Unlock(string key)
    {
        playerUnlocks.Add(key);
    }

    public string PickConversation(string charName)
    {
        if (charName == "lou")
        {
            return "intro";
        }
        else if (charName == "amy")
        {
            if (!playerUnlocks.Contains("wreck-table"))
            {
                return "amy";
            }
            else if (!playerUnlocks.Contains("dark-day"))
            {
                return "amy-canallers";
            }
            else
            {
                return "amy-newspaper";
            }
        }
        else if (charName == "rusty")
        {
            return "shipbuilder";
        }
        return null;
    }
}
