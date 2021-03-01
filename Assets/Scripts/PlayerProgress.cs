using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;

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
    private List<DocumentButton> documentButtons = new List<DocumentButton>();
    private List<NotificationSymbol> notificationSymbols = new List<NotificationSymbol>();
    private List<PhotoSlot> photoSlots = new List<PhotoSlot>();

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
        photoSlots.Add(target);
        UpdatePhoto(target);
    }

    private void UpdatePhoto(PhotoSlot target)
    {
        if (IsUnlocked(target.unlockKey))
        {
            target.SetUnlocked();
        }
        else
        {
            target.SetLocked();
        }
    }

    public void RegisterNotification(NotificationSymbol symbol)
    {
        notificationSymbols.Add(symbol);
        UpdateNotification(symbol);
    }

    private void UpdateNotification(NotificationSymbol symbol)
    {
        bool showNotification = false;
        if (symbol.notificationKey == "any-contact")
        {
            showNotification = HasConversation("lou") || HasConversation("amy") || HasConversation("rusty");
        }
        else if (symbol.notificationKey == "lou")
        {
            showNotification = HasConversation("lou");
        }
        else if (symbol.notificationKey == "amy")
        {
            showNotification = HasConversation("amy");
        }
        else if (symbol.notificationKey == "rusty")
        {
            showNotification = HasConversation("rusty");
        }
        symbol.gameObject.SetActive(showNotification);
    }

    public void SetDocumentPresence(DocumentButton target)
    {
        documentButtons.Add(target);
        UpdateDocumentPresence(target);
    }

    private void UpdateDocumentPresence(DocumentButton target)
    {
        if (playerUnlocks.Contains(target.targetKey))
        {
            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.interactable = true;
            }
            target.GetComponentInChildren<TextMeshProUGUI>().text = target.originalText;
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
        UpdateLockedObjects();
    }

    private void UpdateLockedObjects()
    {
        foreach (var button in documentButtons)
        {
            UpdateDocumentPresence(button);
        }
        foreach (var symbol in notificationSymbols)
        {
            UpdateNotification(symbol);
        }
        foreach (var photo in photoSlots)
        {
            UpdatePhoto(photo);
        }
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

    public bool IsUnlocked(string key)
    {
        return key == null || key == "" || playerUnlocks.Contains(key);
    }

    public void Unlock(string key)
    {
        playerUnlocks.Add(key);
        UpdateBubble();
        UpdateLockedObjects();
    }

    public void ClearRegistrations()
    {
        documentButtons.Clear();
        notificationSymbols.Clear();
        photoSlots.Clear();
    }

    private bool HasConversation(string charName)
    {
        return PickConversation(charName, out string _) != null;
    }

    public string PickConversation(string charName, out string bubble)
    {
        bubble = null;
        if (charName == "lou")
        {
            if (!IsUnlocked("intro-transcript"))
            {
                return "intro";
            }
            else if (!ChapterComplete())
            {
                bubble = "Nothing I need from Lou right now.";
                return null;
            }
            else
            {
                return "ending";
            }
        }
        else if (charName == "amy")
        {
            if (!shipLog.ContainsKey("LocationBox"))
            {
                bubble = "I need to figure out where the wreck is before I call Amy.";
                return null;
            }
            else if (!IsUnlocked("wreck-table"))
            {
                return "amy";
            }
            else if (!shipLog.ContainsKey("NameBox"))
            {
                bubble = "Nothing I need from Amy right now.";
                return null;
            }
            else if (!IsUnlocked("dark-day"))
            {
                return "amy-newspaper";
            }
            else
            {
                bubble = "Nothing I need from Amy right now.";
                return null;
            }
        }
        else if (charName == "rusty")
        {
            if (!shipLog.ContainsKey("TypeBox"))
            {
                bubble = "Nothing I need from Rusty right now.";
                return null;
            }
            else if (!IsUnlocked("rusty-transcript"))
            {
                return "shipbuilder";
            }
            else
            {
                bubble = "Nothing I need from Rusty right now.";
                return null;
            }
        }
        return null;
    }

    private string CurrentThought()
    {
        // stuff on non-document scenes
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ShipMechanics")
        {
            if (!playerUnlocks.Contains("sonar-complete"))
            {
                return "Use your sonar to find the ship!";
            }
            else
            {
                return "Yes! There it is!";
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LaSalleTestScene_RealtimeLighting")
        {
            if (!playerUnlocks.Contains("photo-birds-eye"))
            {
                return "Better get some pictures of the ship.\nI'll start with a picture of the ship from above.";
            }
            else if (!playerUnlocks.Contains("photo-iron-knees"))
            {
                return "Great! Now, I need to see if the ship has any special feature that can help identify it.";
            }
            else
            {
                return "Got everything I need. Time to head back!";
            }
        }

        // intro sequence, before going off to the sonar
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

        // coming back from the dive
        if (!playerUnlocks.Contains("photo-birds-eye"))
        {
            return "I need to go get some photos of the ship!";
        }
        if (!playerUnlocks.Contains("verified-canaller"))
        {
            return "I have a photo of the shape of the ship.\nCan I figure out what type of ship it is?";
        }
        if (!shipLog.ContainsKey("TypeBox"))
        {
            return "Now I can fill in what type of ship it is!";
        }
        if (!playerUnlocks.Contains("rusty-transcript"))
        {
            return "Hmm. The list had two canallers. Which one is it?\nI wonder if the ship expert I know has any clues that might help.";
        }

        // after dialog with Rusty
        if (!playerUnlocks.Contains("photo-iron-knees"))
        {
            return "I need to go get a photo of the ship's knees!";
        }
        if (!playerUnlocks.Contains("verified-loretta"))
        {
            return "I took a photo of the ship's knees.\nWhich of the pictures Rusty sent matches the photo?";
        }
        if (!shipLog.ContainsKey("NameBox"))
        {
            return "Now I can fill in the name of the ship!";
        }
        if (!playerUnlocks.Contains("newspaper"))
        {
            return "Now that I know it's the Loretta, I should call the Archivist\nand see if she knows anything else about it!";
        }

        if (ChapterComplete())
        {
            return "I've filled in everything!\nI should call Lou and let her know!";
        }

        return null;
    }

    public bool ChapterComplete()
    {
        return shipLog.ContainsKey("LocationBox")
            && shipLog.ContainsKey("TypeBox")
            && shipLog.ContainsKey("NameBox")
            && shipLog.ContainsKey("FeatureBox")
            && shipLog.ContainsKey("CauseBox")
            && shipLog.ContainsKey("CargoBox")
            && shipLog.ContainsKey("SecretBox");
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
