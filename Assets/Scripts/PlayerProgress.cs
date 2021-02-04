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
    private ThoughtBubble bubble;
    private string temporaryThought;

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
            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.interactable = false;
            }
            target.GetComponentInChildren<TextMeshProUGUI>().text = "?";
        }
    }

    public void ShipOutButton(ShipOutButton button)
    {
        button.GetComponent<Button>().interactable = CanShipOut();
    }

    public bool CanShipOut()
    {
        return playerUnlocks.Contains("wreck-table");
    }

    public void DropInfo(InfoDropTarget target, InfoEntry info)
    {
        shipLog[target.targetKey] = info;
        FillInfo(target);
        UpdateBubble();
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
        UpdateBubble();
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

    private string CurrentThought()
    {
        if (!playerUnlocks.Contains("intro-transcript"))
        {
            if (bubble != null && bubble.gameObject.scene.name != "OfficeDesk")
            {
                // fall through for testing purposes
            }
            else
            {
                return "Oh, a notification!";
            }
        }
        if (!(shipLog.TryGetValue("LocationBox", out InfoEntry val) && val.infoKey == "coords"))
        {
            return "Let's see. Where's that GPS location?";
        }
        if (!playerUnlocks.Contains("wreck-table"))
        {
            return "That's right off Rawley Point!\nThere are a bunch of ships that went down around there.\nBetter call the archivist and get a list.";
        }
        if (!playerUnlocks.Contains("been-to-sonar"))
        {
            return "Perfect!\nThe wreck didn’t look too deep. I’ll be able to use my normal sonar and dive suit.\nTime to ship out!";
        }
        return null;
    }

    public void SetThoughtBubble(ThoughtBubble newBubble)
    {
        bubble = newBubble;
        UpdateBubble();
    }

    public void ClearThoughtBubble(ThoughtBubble oldBubble)
    {
        if (bubble == oldBubble)
        {
            bubble = null;
        }
    }

    private void UpdateBubble()
    {
        if (bubble == null)
        {
            return;
        }
        string thought = CurrentThought();
        if (temporaryThought != null)
        {
            thought = temporaryThought;
        }
        if (thought == null)
        {
            bubble.gameObject.SetActive(false);
        }
        else
        {
            bubble.GetComponentInChildren<TextMeshProUGUI>().text = thought;
            bubble.gameObject.SetActive(true);
        }
    }

    public void TemporaryBubble(string thought)
    {
        temporaryThought = thought;
        UpdateBubble();
        StartCoroutine(ResetTemporaryThought(thought));
    }

    IEnumerator ResetTemporaryThought(string thought)
    {
        yield return new WaitForSeconds(3);
        if (temporaryThought == thought)
        {
            temporaryThought = null;
            UpdateBubble();
        }
    }
}
