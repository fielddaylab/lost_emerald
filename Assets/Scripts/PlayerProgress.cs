using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;
using System;

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
    private List<CheckboxSymbol> checkboxSymbols = new List<CheckboxSymbol>();
    private List<PhotoSlot> photoSlots = new List<PhotoSlot>();
    private ShipOutButton shipOutButton;
    private string divePerspective;

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

    public void RegisterCheckbox(CheckboxSymbol symbol)
    {
        checkboxSymbols.Add(symbol);
        UpdateCheckbox(symbol);
    }

    private void UpdateCheckbox(CheckboxSymbol symbol)
    {
        if (symbol.checkboxKey == "dive-photo" && IsUnlocked("photo-birds-eye"))
        {
            symbol.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon-check");
        }

        if (symbol.checkboxKey == "defining-feature" && IsUnlocked("photo-iron-knees"))
        {
            symbol.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon-check");
        }
    }

    private void UpdateNotification(NotificationSymbol symbol)
    {
        bool showNotification = false;
        if (symbol.notificationKey == "lou-intro")
        {
            showNotification = !IsUnlocked("intro-transcript");
        }
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
        else if (symbol.notificationKey == "lou-coords-transcript")
        {
            showNotification = !IsUnlocked("viewed-intro-transcript");
        }
        else if (symbol.notificationKey == "any-document" && !(IsUnlocked("verified-loretta") && IsUnlocked("verified-canaller")))
        {
            if (IsUnlocked("verified-canaller"))
            {
                showNotification = IsUnlocked("photo-iron-knees-dragged");
            }
            else
            {
                showNotification = IsUnlocked("photo-birds-eye-dragged");
            }
        }
        else if (symbol.notificationKey == "wrecks" && !IsUnlocked("viewed-wreck-table"))
        {
            showNotification = IsUnlocked("wreck-table");
        }
        else if (symbol.notificationKey == "ship-out" && !IsUnlocked("photo-birds-eye"))
        {
            showNotification = IsUnlocked("viewed-wreck-table");
        }
        else if (symbol.notificationKey == "evidence-builder" && IsUnlocked("photo-birds-eye") && !IsUnlocked("EvidenceBuilder"))
        {
            if (IsUnlocked("rusty-transcript"))
            {
                showNotification = !IsUnlocked("verified-loretta");
            }
            else
            {
                showNotification = !IsUnlocked("verified-canaller");
            }
        }
        else if (symbol.notificationKey == "rusty-convo-doc")
        {
            showNotification = IsUnlocked("photo-iron-knees-dragged") && !IsUnlocked("verified-loretta");
        }
        else if (symbol.notificationKey == "image" && IsUnlocked("photo-birds-eye"))
        {
            if (IsUnlocked("rusty-transcript") && !IsUnlocked("photo-iron-knees-dragged"))
            {
                showNotification = !IsUnlocked("verified-loretta");
            }
            else
            {
                showNotification = !IsUnlocked("verified-canaller") && !IsUnlocked("photo-birds-eye-dragged");
            }
        }
        else if (symbol.notificationKey == "evidence")
        {
            showNotification = (IsUnlocked("verified-canaller") && !shipLog.ContainsKey("TypeBox")) || (IsUnlocked("verified-loretta") && !shipLog.ContainsKey("NameBox"));
        }
        else if (symbol.notificationKey == "perspective")
        {
            showNotification = !IsUnlocked("photo-iron-knees") && IsUnlocked("photo-birds-eye") && !IsUnlocked("CAMERA_SIDE");
        }
        else if (symbol.notificationKey == "bird-view-thought")
        {
            showNotification = !IsUnlocked("photo-birds-eye");
        }
        else if (symbol.notificationKey == "dive-ready")
        {
            showNotification = IsUnlocked("sonar-complete");
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
        if (IsUnlocked(target.targetKey))
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
        shipOutButton = button;
        UpdateShipOutButton();
    }

    private void UpdateShipOutButton()
    {
        if (shipOutButton)
        {
            shipOutButton.GetComponent<Button>().interactable = CanShipOut();
        }
    }

    public bool CanShipOut()
    {
        return IsUnlocked("viewed-wreck-table");
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
        foreach (var symbol in checkboxSymbols)
        {
            UpdateCheckbox(symbol);
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
        UpdateShipOutButton();
    }

    public void ClearRegistrations()
    {
        documentButtons.Clear();
        notificationSymbols.Clear();
        photoSlots.Clear();
        shipOutButton = null;
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
            else if (!IsUnlocked("informed-lou"))
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
            if (!IsUnlocked("ship-on-lake"))
            {
                return "The red dot shows the GPS location of the ship";
            }
            else if (!IsUnlocked("sonar-complete") && IsUnlocked("ship-on-lake"))
            {
                return "Use your sonar to find the ship!";
            }
            else if (IsUnlocked("sonar-complete") )
            {
                return "Yes! There it is!";
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LaSalleTestScene_RealtimeLighting")
        {
            if (!IsUnlocked("photo-birds-eye"))
            {
                return "Better get a picture of the whole wreck while I’m here.\nI'll start with a picture of the ship from above.";
            }
            else if (!IsUnlocked("photo-iron-knees"))
            {
                return "Welp, better dive down further";
            }
            else
            {
                return "I have all the pictures I need. Time to head back to the office!";
            }
        }

        // intro sequence, before going off to the sonar
        if (!IsUnlocked("intro-transcript"))
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
            if (!IsUnlocked("viewed-intro-transcript"))
            {
                return "Let's see. Where's that GPS location?";
            }
            else
            {
                return "Now I can drag the coordinates over to the Location field.";
            }
        }
        if (!IsUnlocked("wreck-table"))
        {
            return "That's right off Rawley Point! There are a bunch of ships\nthat went down around there. Better call the archivist and get a list.";
        }
        if (!IsUnlocked("been-to-sonar"))
        {
            if (!IsUnlocked("viewed-wreck-table"))
            {
                return "The List of Wrecks should help me narrow things down.";
            }
            else
            {
                return "It has to be one of these 5 ships.\nTime to Ship Out!";
            }
        }

        // coming back from the dive
        if (!IsUnlocked("photo-birds-eye"))
        {
            return "I need to go get some photos of the ship!";
        }
        if (!IsUnlocked("verified-canaller"))
        {
            return "I have a photo of the shape of the ship.\nCan I figure out what type of ship it is?";
        }
        if (!shipLog.ContainsKey("TypeBox"))
        {
            return "Now I can fill in what type of ship it is!";
        }
        if (!IsUnlocked("rusty-transcript"))
        {
            return "Hmm. The list had two canallers. Which one is it?\nI wonder if the ship expert I know has any clues that might help.";
        }

        // after dialog with Rusty
        if (!IsUnlocked("photo-iron-knees"))
        {
            return "I need to go get a photo of the ship's knees!";
        }
        if (!IsUnlocked("verified-loretta"))
        {
            return "I took a photo of the ship's knees.\nWhich of the pictures Rusty sent matches the photo?";
        }
        if (!shipLog.ContainsKey("NameBox"))
        {
            return "Now I can fill in the name of the ship!";
        }
        if (!IsUnlocked("newspaper"))
        {
            return "Now that I know it's the Loretta, I should call the Archivist\nand see if she knows anything else about it!";
        }

        if (ChapterComplete() && !IsUnlocked("informed-lou"))
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
            if (thought == "Let's see. Where's that GPS location?" ||
                thought == "The List of Wrecks should help me narrow things down." ||
                thought == "I have a photo of the shape of the ship.\nCan I figure out what type of ship it is?")
            {
                bubble.animate = true;
            }
        }
    }

    public void TemporaryBubble(string thought)
    {
        temporaryThought = thought;
        UpdateBubble();
        StartCoroutine(ResetTemporaryThought(thought));
    }

    public void UnlockTabEvidenceBuilder(string tabPanel)
    {
        if (tabPanel == "EvidenceBuilder")
        {
            Unlock(tabPanel);
        }
        else if (tabPanel == "ScrollOverview")
        {
            playerUnlocks.Remove("EvidenceBuilder");
        }
    }

    public void SetDivePerspective(string cameraState)
    {
        playerUnlocks.Remove(divePerspective);
        divePerspective = cameraState;
        Unlock(cameraState);
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
