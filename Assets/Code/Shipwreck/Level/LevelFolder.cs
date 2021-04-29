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

        private bool IsComplete = false;

        public void Start() {

            button.onClick.RemoveAllListeners();

            if(LevelHelper.CurrentScene() == "LevelEndCutscene"){
                m_LevelID = PlayerProgress.instance.PrevLevel;
            }
            m_Text.SetText(m_LevelID);

            IsComplete = PlayerProgress.instance.IsLevelComplete(m_LevelID);

            if(IsComplete) {
                m_CaseStatus.gameObject.SetActive(true);
                if(LevelHelper.CurrentScene() != "LevelEndCutscene") {
                    button.onClick.AddListener(ReloadLevel);
                }
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