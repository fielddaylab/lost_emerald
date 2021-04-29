using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;
using System;
using Shipwreck.Level;
using Shipwreck.Document;

namespace Shipwreck
{
    public class PlayerProgress : MonoBehaviour
    {
        public struct InfoEntry
        {
            public string infoKey;
            public string infoDisplay;
            public string sourceDisplay;
        }

        [SerializeField] private DocumentReloader m_Reloader = null;

        public static PlayerProgress instance;
        private Dictionary<string, InfoEntry> shipLog = new Dictionary<string, InfoEntry>();
        private Dictionary<string, LevelSaveData> m_SaveData = new Dictionary<string, LevelSaveData>();
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
        private string prevSceneName;
        private string currentLevelID = "loretta";
        public string PrevLevel { get; set; }
        private LevelBase currentLevel = new LevelLoretta();
        private List<string> lockedLevels = new List<string>();

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

        #region SaveData

        public string GetCurrentLevel()
        {
            return currentLevelID;
        }

        public void LoadLevel(string levelID)
        {
            currentLevelID = levelID;
            if (currentLevelID == "loretta")
            {
                currentLevel = new LevelLoretta();
                SaveData();
            }
            else if (currentLevelID == "level2")
            {
                currentLevel = new Level2();
                SaveData();
            }
            shipLog.Clear();
            playerUnlocks.Clear();

            
        }

        public void ReloadLevel(string levelID) {
            if(m_SaveData.TryGetValue(levelID, out LevelSaveData save)) {
                if(m_SaveData.TryGetValue(currentLevelID, out LevelSaveData currentSave)) {
                    currentSave.ShipLog = shipLog;
                    currentSave.Unlocks = playerUnlocks;
                }
                else {
                    m_SaveData.Add(currentLevelID, new LevelSaveData(shipLog, playerUnlocks));
                }
                shipLog = save.ShipLog;
                playerUnlocks = save.Unlocks;
                m_Reloader?.ReloadDocument();
                currentLevelID = levelID;
            }
            else {
                Debug.Log("error on reloading!!");
            }

        }

        public bool IsGameStart() {
            return m_SaveData.Keys.Count == 0;
        }

        public bool SetComplete() {
            m_SaveData.TryGetValue(currentLevelID, out LevelSaveData save);
            currentLevelID = null;
            save.Complete();
            return save.IsCompleted();
        }

        public void AddSaveData(string levelName) {
            if(m_SaveData.TryGetValue(levelName, out LevelSaveData save)) {
                if(save.ShipLog != shipLog) save.ShipLog = shipLog;
                if(save.Unlocks != playerUnlocks) save.Unlocks = playerUnlocks;
                save.Complete();
            }
        }

        public bool IsLevelComplete(string levelID) {
            if(m_SaveData.TryGetValue(levelID, out LevelSaveData save)) {
                return save.IsCompleted();
            }
            return false;
        }

        public void SaveData() {
            if(m_SaveData.TryGetValue(currentLevelID, out LevelSaveData save)) {
                if(save.IsCompleted()) {
                    shipLog = save.ShipLog;
                    playerUnlocks = save.Unlocks;
                }
            }
            else {
                m_SaveData.Add(currentLevelID, new LevelSaveData());
            }
        }

        #endregion //Save Data

        #region Data Handling

        public bool FillInfo(InfoDropTarget target, bool initial=false)
        {
            // if(initial) {
            //     SaveData();
            // }
            if (shipLog.TryGetValue(target.targetKey, out InfoEntry entry))
            {
                target.Fill(entry.infoDisplay, "From: " + entry.sourceDisplay);
                return true;
            }
            else
            {
                target.infoLabel.text = "Unknown";
                target.sourceLabel.text = "";
                return false;

            }
        }

        public void SetPhotoPresence(PhotoSlot target)
        {
            photoSlots.Add(target);
            UpdatePhoto(target);
        }

        public void UpdatePhoto(PhotoSlot target)
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

        #endregion //Data Handling

        #region Level

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
            bool check = currentLevel.CheckboxStatus(this, symbol.checkboxKey);
            if (check)
            {
                symbol.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon-check");
            }
        }

        private void UpdateNotification(NotificationSymbol symbol)
        {
            bool showNotification = currentLevel.NotificationStatus(this, symbol.notificationKey);
            symbol.gameObject.SetActive(showNotification);
        }

        #endregion // Level

        #region Documents

        public void SetDocumentPresence(DocumentButton target)
        {
            documentButtons.Add(target);
            UpdateDocumentPresence(target);
        }

        public void UpdateDocumentPresence(DocumentButton target)
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

        #endregion //Documents

        public void ShipOutButton(ShipOutButton button)
        {
            shipOutButton = button;
            UpdateShipOutButton();
        }

        private void UpdateShipOutButton()
        {
            if (shipOutButton)
            {
                bool canShipOut = currentLevel.CanShipOut(this);
                shipOutButton.GetComponent<Button>().interactable = canShipOut;
                if(canShipOut) {
                    if(!IsUnlocked("loretta-ship-out")) {
                        Unlock("loretta-ship-out");
                    }
                }
            }
        }

        public void DropInfo(InfoDropTarget target, InfoEntry info)
        {
            shipLog[target.targetKey] = info;
            FillInfo(target);
            UpdateBubble();
            UpdateShipOutButton();
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

        public bool FilledLog(string key)
        {
            return shipLog.ContainsKey(key);
        }

        public bool IsUnlocked(string key)
        {
            return key == null || key == "" || playerUnlocks.Contains(key);
        }

        public void Unlock(string key)
        {
            playerUnlocks.Add(key);
            if (!key.Equals("EvidenceBuilder"))
            {
                Logging.instance?.LogPlayerUnlock(key);
            }
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

        public bool HasConversation(string charName)
        {
            return PickConversation(charName, out string _) != null;
        }

        public string PickConversation(string charName, out string bubble)
        {
            return currentLevel.PickConversation(this, charName, out bubble);
        }

        private string CurrentThought()
        {
            return currentLevel.CurrentThought(this);
        }

        public bool ChapterComplete()
        {
            return currentLevel.ChapterComplete(this);
        }

        public void SetThoughtBubble(ThoughtBubble newBubble)
        {
            bubble = newBubble;
            UpdateBubble();
        }

        public ThoughtBubble GetThoughtBubble()
        {
            return bubble;
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
                TextMeshProUGUI textMesh = bubble.GetComponentInChildren<TextMeshProUGUI>();
                textMesh.text = thought;
                Vector2 size = textMesh.GetPreferredValues(600, 50);
                RectTransform rect = bubble.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(size.x + 40, size.y + 20);

                bubble.gameObject.SetActive(true);
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

        public string GetDivePerspective()
        {
            return divePerspective;
        }

        public void SetPrevSceneName(string prevScene)
        {
            prevSceneName = prevScene;
        }

        public string GetPrevSceneName()
        {
            return prevSceneName;
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
}
