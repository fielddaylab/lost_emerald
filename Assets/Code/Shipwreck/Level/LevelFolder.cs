using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Shipwreck;
using TMPro;
using Shipwreck.Scene;

namespace Shipwreck.Level
{
    public class LevelFolder : SceneSwitch 
    {

        public Button button = null; // will be null for endscene
        public TextMeshProUGUI m_Text = null;
        public string m_LevelID = null;
        public Transform m_CaseStatus = null;

        private bool IsUnlocked = false;
        private bool IsComplete = false;
        private string m_DisplayID = null;

        public void Start() {

            button.onClick.RemoveAllListeners();

            var playerData = PlayerProgress.instance;

            SetDisplay();

            m_Text.SetText(m_DisplayID.ToUpper());

            IsComplete = playerData.IsLevelComplete(m_LevelID);

            UpdateButton();

        }

        private void SetDisplay() {

            var playerData = PlayerProgress.instance;

            if(LevelHelper.CurrentScene() == "LevelEndCutscene"){
                m_LevelID = playerData.PrevLevel;
                m_DisplayID = m_LevelID;
            }
            else if(m_LevelID != null) {
                if(playerData.IsLevelComplete(m_LevelID) 
                || (playerData.GetCurrentLevel() == m_LevelID && playerData.FilledLog("NameBox")))  {
                    //filled log refers to document 1, so level 2 will pop up. need to work on that.
                    m_DisplayID = m_LevelID;
                }
                else 
                {
                    m_DisplayID = "?";
                }
            }
        }

        private void UpdateButton() {
            if(IsComplete) {
                m_CaseStatus.gameObject.SetActive(true);
                if(LevelHelper.CurrentScene() != "LevelEndCutscene") {
                    button.onClick.AddListener(ReloadLevel);
                }
            }
            else if(PlayerProgress.instance.GetCurrentLevel() == "level2") {
                button.onClick.AddListener(GotoLevel2);
            }
            else {
                m_CaseStatus.gameObject.SetActive(false);
                button.onClick.AddListener(GotoDesk);
            }
            
            button.interactable = (IsComplete || PlayerProgress.instance.GetCurrentLevel() == m_LevelID);
        }

        private void ReloadLevel(){
            string relLevelID = m_LevelID == null ? PlayerProgress.instance.PrevLevel : m_LevelID;
            GotoDocuments(relLevelID, true);
        }

        public void StartLevel()
        {
            PlayerProgress.instance.LoadLevel(m_LevelID);
            if(IsComplete) Logging.instance?.LogMissionStart(m_LevelID);
            GotoDocuments(m_LevelID);
        }

        public void Complete() {
            IsComplete = true;
            m_CaseStatus.gameObject.SetActive(true);

        }

        public bool IsCompleted() {
            return IsComplete;
        }

        public bool IsLevel(string level) {
            return m_LevelID == level;
        }

    }
}